# Game Score Server

Serveur Django REST pour gérer les scores du jeu Flappy Bird.

## Configuration

Le projet utilise Docker et Docker Compose avec des profils pour gérer les environnements de développement et de production.

### Prérequis

- Docker
- Docker Compose v2+

### Structure des environnements

Le projet dispose de deux environnements :
- Développement (dev) : Port 8000
- Production (prod) : Port 8001

## Utilisation

### Environnement de développement

Pour lancer le serveur en mode développement :

```bash
docker compose --profile dev up
```

### Environnement de production

Pour lancer le serveur en mode production :

```bash
docker compose --profile prod up
```
