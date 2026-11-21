# TeamLink Backend

## Kas įgyvendinta

- **BR-01**: registracija (vardas, pavardė, el. paštas, slaptažodis ir patvirtinimas), prisijungimas, 15 min. access JWT, 30 dienų refresh sesija, atsijungimas; bcrypt slaptažodžių hash; 5 neteisingų bandymų blokavimas 15 min.
- **BR-02**: savo profilio peržiūra ir redagavimas; vartotojų paieška pagal vardą/pavardę, reikalinga naujam pokalbiui.
- **BR-03**: vienas su vienu pokalbio sukūrimas, pokalbių sąrašas ir pokalbio dalyviai, tik autorizuotiems vartotojams. Pakartotinai kuriant tą patį privatų pokalbį, grąžinamas esamas.

**Dar neįgyvendinta:** BR-04–BR-20, tarp jų žinučių siuntimas (BR-05), istorija (BR-06), grupės, WebSocket, žinučių redagavimas/trynimas, reakcijos, failai ir pranešimai. BR-09 reikalauja klientų ir bus vertinamas vėliau. Nei produkcinis saugumas, nei visų NFR našumas ir testų aprėptis dar nėra patikrinti.

## Paleidimas

1. Įsidiekite **.NET 8 SDK** ir **Docker** su Docker Compose.
2. Terminale atsidarykite šį aplanką.
3. Paleiskite PostgreSQL:
   ```bash
   docker compose up -d
   ```
4. Nustatykite aplinkos kintamuosius iš `.env.example`. **`.env` failas nėra automatiškai įkeliamas ASP.NET Core**: per VS Code / shell turite eksportuoti juos į paleidžiamą procesą. Pvz. Linux/macOS (bash):
   ```bash
   export ConnectionStrings__Default='Host=localhost;Port=5433;Database=teamlink_stage1;Username=teamlink;Password=change_me'
   export Jwt__Key='replace-with-a-random-secret-of-at-least-32-bytes'
   export Jwt__Issuer='TeamLink'
   export Jwt__Audience='TeamLinkClients'
   export ASPNETCORE_ENVIRONMENT=Development
   ```
   Windows PowerShell atveju vietoje `export` naudokite `$env:Jwt__Key = '...'` ir analogiškai kitus kintamuosius.
5. Paleiskite:
   ```bash
   dotnet restore
   dotnet run --urls http://localhost:5080
   ```
6. Atidarykite `http://localhost:5080/swagger` ir pabandykite API.

**Pastaba:** tik `Development` režimu pirmo paleidimo metu naudojama `EnsureCreated`, kuri sukuria pradines lenteles **tuščioje** PostgreSQL duomenų bazėje. Tai nesuteikia migracijų: struktūros pakeitimams reikia EF migrations. Naudokite atskirą naują DB, ne seną `TeamLink` duomenų bazę iš ankstesnio projekto. Prieš viešą diegimą įjunkite HTTPS, saugų cookie ir perkelkite visus slaptus raktus į saugiai valdomą konfigūraciją.

## REST API

| Metodas | Kelias | BR |
| --- | --- | --- |
| POST | `/api/auth/register` | BR-01 |
| POST | `/api/auth/login` | BR-01 |
| POST | `/api/auth/refresh` | BR-01 |
| POST | `/api/auth/logout` | BR-01 |
| GET | `/api/users/me` | BR-02 |
| PATCH | `/api/users/me` | BR-02 |
| GET | `/api/users?q=Jon` | BR-02/03 |
| POST | `/api/conversations/private` | BR-03 |
| GET | `/api/conversations` | BR-03 |
| GET | `/api/conversations/{id}/participants` | BR-03 |
| GET | `/health` | patikra |

Registracija JSON:
```json
{ "name": "Jonas", "surname": "Jonaitis", "email": "jonas@example.com", "password": "Slaptazodis123", "confirmPassword": "Slaptazodis123" }
```
Prisijungimas JSON:
```json
{ "email": "jonas@example.com", "password": "Slaptazodis123", "platform": "WEB" }
```
Pokalbio sukūrimas (įrašykite egzistuojančio kito vartotojo ID):
```json
{ "recipientId": "00000000-0000-0000-0000-000000000001" }
```
Saugomiems maršrutams pridėkite `Authorization: Bearer <accessToken>`.

## GitHub

Prieš įkeldami patikrinkite, kad nebūtų įkelti tikrieji JWT raktai, slaptažodžiai ir `.env` failas. Į bendrą komandos repozitoriją geriausia įkelti šį turinį kaip `backend/`, atlikti atskirą commit ir `push` savo šakai (`feature/backend-stage1`), po to atidaryti pull request.
