# TeamLink monorepo

## Backend

Follow the [Backend README](Backend/README.md) for instructions.

## Frontend

Follow the [Frontend README](Frontend/README.md) for instructions.

## Containers

Backend and Frontend are treated as separate services and should be prepared to run in (separate) Docker containers. The Compose file is at the repository root. Separate Dockerfiles are placed in the respective directories.

## Run Applications

### Manually

Please refer to the README.md files in the respective directories for instructions on how to run the applications manually.

### Using Docker Compose

You can run or deploy the applications using Docker Compose from the repository root. It is configured to run both applications and a PostgreSQL database.

You can always choose to run only the backend or frontend by specifying the service name in the `docker compose` command.

Components are named as follows:
- `db` - PostgreSQL database
- `api` - Backend API
- `frontend` - reserved for the frontend application

Copy `.env.example` to `.env` and edit it to configure the applications/components. Shell environment variables override values in `.env`.

Start everything:
```powershell
docker compose up -d --build
```

Build or start only specific services (without touching the others):
```powershell
docker compose up -d --build --no-deps COMPONENT_NAME [ANOTHER_COMPONENT_NAME ...]
```

`--no-deps` prevents starting the dependencies of the specified service. If you want to start a service and its dependencies (e.g., `api` will start `db`), omit `--no-deps`.

View logs or stop just one service
```powershell
docker compose logs -f COMPONENT_NAME
docker compose stop COMPONENT_NAME
```