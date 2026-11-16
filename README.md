# 📚 Gestionale Biblioteca - REST API (ASP.NET Core & JWT)

Backend RESTful sviluppato con **ASP.NET Core 9** per la gestione di una biblioteca: catalogo libri, anagrafica utenti con ruoli, prestiti/restituzioni e autenticazione stateless tramite **JSON Web Token (JWT)**.

Il progetto è la riscrittura in C# di una precedente versione in Spring Boot, realizzata a scopo di studio.

---

## 🚀 Tecnologie utilizzate

* **C# / .NET 9**
* **ASP.NET Core Web API** (controller)
* **Entity Framework Core 9** con driver **Pomelo** (MySQL) e migrazioni *code-first*
* **Autenticazione JWT Bearer** (`Microsoft.AspNetCore.Authentication.JwtBearer`)
* **BCrypt.Net-Next** (hash delle password)
* **MySQL 8**

---

## 🏗️ Architettura

Architettura a livelli, con DTO per separare ciò che esce dall'API dalle entità del database:

```text
BibliotecaApi/
├── Controllers/   # Endpoint REST (Auth, Book, User, Borrow)
├── Services/      # Logica di business (interfacce I...Service + implementazioni)
├── Dtos/          # Record di ingresso/uscita e wrapper ResponseApi<T>
├── Models/        # Entità EF Core (User, Role, Book, Borrow)
├── Data/          # BibliotecaDbContext (mapping tabelle, indici, ruoli iniziali)
├── Migrations/    # Migrazioni EF Core
└── Program.cs     # Dependency injection, autenticazione, pipeline HTTP
```

Tutte le risposte di successo sono avvolte in un wrapper standard:

```json
{ "status": "SUCCESS", "data": { } }
```

Gli errori usano i codici HTTP appropriati (`400`, `401`, `403`, `404`, `409`).

---

## ⚙️ Configurazione ed avvio

### Prerequisiti

* [.NET SDK 9](https://dotnet.microsoft.com/download)
* MySQL 8 in esecuzione
* Lo strumento delle migrazioni: `dotnet tool install --global dotnet-ef --version 9.0.0`

### 1. Segreti (User Secrets)

Password del database e chiave JWT **non sono nel repository**: si salvano con gli *User Secrets* di .NET. Dalla cartella del progetto:

```bash
dotnet user-secrets init
dotnet user-secrets set "ConnectionStrings:Biblioteca" "Server=localhost;Port=3306;Database=biblioteca_cs;User=root;Password=LA_TUA_PASSWORD;"
dotnet user-secrets set "Jwt:Key" "UNA_STRINGA_CASUALE_DI_ALMENO_32_CARATTERI"
```

Le parti non segrete (`Issuer`, `Audience`, durata del token) sono in `appsettings.json`.

### 2. Creazione del database

Le tabelle e i due ruoli iniziali (`ROLE_USER`, `ROLE_ADMIN`) vengono creati dalle migrazioni:

```bash
dotnet ef database update
```

### 3. Avvio

```bash
dotnet run --launch-profile http
```

L'API risponde su `http://localhost:5044`.

### 4. Creare il primo amministratore

La registrazione pubblica assegna sempre `ROLE_USER`. Per avere un admin, registra un utente con `POST /api/user` e promuovilo da SQL:

```sql
INSERT INTO user_roles (UsersId, RolesId)
SELECT u.Id, r.Id FROM users u, roles r
WHERE u.Username = 'tuo_username' AND r.Name = 'ROLE_ADMIN';
```

---

## 🔐 Autenticazione e sicurezza

1. L'utente effettua il login con `POST /api/auth/login` e riceve un token JWT.
2. Per gli endpoint protetti il token va inviato in ogni richiesta:

   ```http
   Authorization: Bearer <token_jwt>
   ```

3. Il token contiene l'`uuid` dell'utente (`sub`), lo username e i ruoli, e viene validato (firma, issuer, audience, scadenza) a ogni richiesta.

### Ruoli

* `ROLE_USER`: consulta il catalogo, gestisce i **propri** prestiti.
* `ROLE_ADMIN`: gestione completa di libri, utenti e prestiti.

### Scelte di sicurezza

* Le password sono salvate come **hash BCrypt**; l'hash non viene mai restituito dall'API.
* La **registrazione pubblica assegna sempre `ROLE_USER`**: i ruoli non si scelgono dal client.
* Il login risponde `401` allo stesso modo per username inesistente, password errata e utente disattivato.
* Il prestito viene sempre intestato all'**utente del token**, mai a un utente indicato nel body.
* Un utente normale non vede i prestiti degli altri (risposta `404`).
* Segreti fuori dal repository (User Secrets).

---

## 📡 Riepilogo endpoint

### 🔑 Autenticazione (`/api/auth`)

| Metodo | Endpoint | Accesso | Descrizione |
| :--- | :--- | :--- | :--- |
| `POST` | `/api/auth/login` | Pubblico | Verifica le credenziali e restituisce il token JWT (`401` se errate) |

### 👤 Utenti (`/api/user`)

| Metodo | Endpoint | Accesso | Descrizione |
| :--- | :--- | :--- | :--- |
| `POST` | `/api/user` | Pubblico | Registra un utente con `ROLE_USER` (`409` se username o email esistono già) |
| `GET` | `/api/user` | `ROLE_ADMIN` | Elenco degli utenti |
| `GET` | `/api/user/{uuid}` | `ROLE_ADMIN` | Dettaglio di un utente |

### 📖 Libri (`/api/book`)

| Metodo | Endpoint | Accesso | Descrizione |
| :--- | :--- | :--- | :--- |
| `GET` | `/api/book` | Pubblico | Tutti i libri del catalogo |
| `GET` | `/api/book/uuid/{uuid}` | Pubblico | Libro per UUID (`404` se assente) |
| `GET` | `/api/book/isbn/{isbn}` | Pubblico | Libro per ISBN (`404` se assente) |
| `POST` | `/api/book` | `ROLE_ADMIN` | Crea un libro (`201`; `409` se l'ISBN esiste già) |
| `PUT` | `/api/book/{uuid}` | `ROLE_ADMIN` | Modifica un libro (`404`; `409` se l'ISBN appartiene a un altro libro) |
| `DELETE` | `/api/book/{uuid}` | `ROLE_ADMIN` | Elimina un libro (`204`; `404` se assente) |

### 🤝 Prestiti (`/api/borrow`)

| Metodo | Endpoint | Accesso | Descrizione |
| :--- | :--- | :--- | :--- |
| `GET` | `/api/borrow` | Autenticato | Admin: tutti i prestiti. Utente: solo i propri |
| `GET` | `/api/borrow/{uuid}` | Autenticato | Dettaglio di un prestito (un utente solo se è il suo) |
| `POST` | `/api/borrow` | Autenticato | Nuovo prestito per l'utente del token; decrementa di 1 le copie (`409` se non disponibili) |
| `PUT` | `/api/borrow/{uuid}/return` | Autenticato | Registra la restituzione e incrementa di 1 le copie (`409` se già restituito) |
| `DELETE` | `/api/borrow/{uuid}` | `ROLE_ADMIN` | Elimina un prestito |

### Esempi

**Login**

```http
POST /api/auth/login
Content-Type: application/json

{ "username": "mario", "password": "la_tua_password" }
```

**Nuovo prestito** (con il token dell'utente)

```http
POST /api/borrow
Authorization: Bearer <token_jwt>
Content-Type: application/json

{ "bookUuid": "uuid-del-libro" }
```

---

## 🧭 Possibili sviluppi

* Aggiornamento ed eliminazione degli utenti, gestione dei ruoli da parte dell'admin
* Validazione degli input (formato email, lunghezza minima password)
* Impedire l'eliminazione di un libro con prestiti associati
* Controllo di concorrenza sull'ultima copia disponibile
* Gestione globale degli errori e test automatici
