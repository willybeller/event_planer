
# 📅 Event Planner API

  

Une API complète pour la gestion d'événements avec authentification JWT, développée en ASP.NET Core 8.

  

## 🚀 Fonctionnalités

  

### 🔐 Authentification

-  **Inscription** et **connexion** utilisateur

-  **Authentification JWT** avec tokens de 2 heures

-  **Hashage sécurisé** des mots de passe avec BCrypt

-  **Profil utilisateur** avec informations personnelles

  

### 📅 Gestion des événements

-  **Création** d'événements publics ou privés

-  **Modification** et **suppression** par le créateur

-  **Consultation** des événements accessibles

-  **Détails complets** avec participants et statuts

  

### 👥 Système de participants

-  **Invitations par email** pour événements privés

-  **Participation libre** aux événements publics

-  **Gestion des statuts RSVP** (yes/no/maybe/pending)

-  **Liste des participants** avec informations détaillées

  

## 🛠️ Technologies utilisées

  

-  **ASP.NET Core 8.0** - Framework web

-  **Entity Framework Core** - ORM pour base de données

-  **SQLite** - Base de données légère

-  **JWT Bearer** - Authentification par tokens

-  **BCrypt.Net** - Hashage sécurisé des mots de passe

-  **Swagger/OpenAPI** - Documentation API interactive

  

## 📋 Prérequis

  

- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)

- Un éditeur de code (Visual Studio, VS Code, etc.)

  

## 🔧 Installation

  

1.  **Cloner le repository**

```bash

git clone https://github.com/willybeller/event_planner.git

cd event_planner/EventPlannerAPI

```

  

2.  **Restaurer les packages NuGet**

```bash

dotnet restore

```

  

3.  **Configurer la base de données**

```bash

dotnet ef database update

```

> La base de données SQLite sera créée automatiquement au premier lancement

  

4.  **Configurer les paramètres JWT** (optionnel)

Modifier `appsettings.json` :

```json

{

"JwtSettings": {

"SecretKey": "votre-clé-secrète-très-longue-et-sécurisée",

"Issuer": "EventPlannerAPI",

"Audience": "EventPlannerAPI"

}

}

```

  

## 🚀 Démarrage

  

1.  **Lancer l'application**

```bash

dotnet run

```

  

2.  **Accéder à la documentation Swagger**

```

http://localhost:5021/swagger

```

  

3.  **Tester l'API**

- Créer un compte via `/api/auth/signup`

- Se connecter via `/api/auth/login`

- Utiliser le token JWT pour les autres endpoints

  

## 📚 Documentation API

  

### 🔐 Authentification

| Endpoint           | Méthode | Description                           |
|--------------------|---------|---------------------------------------|
| `/api/auth/signup` | POST    | Inscription d'un nouvel utilisateur  |
| `/api/auth/login`  | POST    | Connexion utilisateur                 |
| `/api/auth/me`     | GET     | Profil de l'utilisateur connecté      |

  

### 📅 Événements

| Endpoint           | Méthode | Description                 | Auth |
|--------------------|---------|-----------------------------|----- |
| `/api/events`      | GET     | Liste des événements       | ❌   |
| `/api/events`      | POST    | Créer un événement         | ✅   |
| `/api/events/{id}` | GET     | Détails d'un événement     | ❌   |
| `/api/events/{id}` | PUT     | Modifier un événement      | ✅   |
| `/api/events/{id}` | DELETE  | Supprimer un événement     | ✅   |

  

### 👥 Participants

| Endpoint                        | Méthode | Description                   | Auth |
|---------------------------------|---------|-------------------------------|----- |
| `/api/events/{id}/invite`       | POST    | Inviter un participant        | ✅   |
| `/api/events/{id}/join`         | POST    | Rejoindre un événement       | ✅   |
| `/api/events/{id}/rsvp`         | PATCH   | Modifier son statut RSVP      | ✅   |
| `/api/events/{id}/participants` | GET     | Liste des participants        | ✅   |

  

## 📊 Modèles de données

  

### 👤 Utilisateur

```json

{

"id": 1,

"name": "Jean Dupont",

"email": "jean@example.com",

"createdAt": "2024-01-15T10:30:00Z"

}

```

  

### 📅 Événement

```json

{

"id": 1,

"title": "Conférence Tech 2024",

"description": "Conférence sur les dernières technologies",

"date": "2024-06-15T14:30:00Z",

"location": "Centre de conférences, Paris",

"maxParticipants": 100,

"isPublic": true,

"creatorId": 1,

"participants": []

}

```

  

### 👥 Participant

```json

{

"email": "participant@example.com",

"rsvpStatus": "yes",

"isAdmin": false,

"user": {

"id": 2,

"name": "Marie Martin"

}

}

```

  

## 🔑 Authentification JWT

  

### Obtenir un token

1.  **S'inscrire** ou **se connecter**

2.  **Copier le token** de la réponse

3.  **Ajouter le header** : `Authorization: Bearer {votre-token}`

  

### Durée de validité

-  **Tokens JWT** : 2 heures

-  **Renouvellement** : Reconnexion nécessaire après expiration

  

## 🗃️ Structure du projet

  

```

EventPlannerAPI/

├── Controllers/ # Contrôleurs API

│ ├── AuthController.cs

│ ├── EventsController.cs

│ └── ParticipantsController.cs

├── Data/ # Contexte de base de données

│ └── ApplicationDbContext.cs

├── DTOs/ # Objets de transfert de données

│ ├── AuthDto.cs

│ ├── EventDto.cs

│ └── ParticipantDto.cs

├── Models/ # Modèles de données

│ ├── User.cs

│ ├── Event.cs

│ └── EventParticipant.cs

├── Services/ # Services métier

│ ├── AuthService.cs

│ ├── EventService.cs

│ └── ParticipantService.cs

└── Program.cs # Point d'entrée de l'application

```

  

## 🔧 Configuration

  

### appsettings.json

```json

{

"ConnectionStrings": {

"DefaultConnection": "Data Source=eventplanner.db"

},

"JwtSettings": {

"SecretKey": "your-super-secret-key-here-make-it-very-long-and-secure",

"Issuer": "EventPlannerAPI",

"Audience": "EventPlannerAPI"

},

"Logging": {

"LogLevel": {

"Default": "Information",

"Microsoft.AspNetCore": "Warning"

}

}

}

```

  

## 🚦 Statuts RSVP

| Statut    | Description                                    |
|-----------|------------------------------------------------|
| `pending` | En attente de réponse (nouvelles invitations) |
| `yes`     | Participera à l'événement                      |
| `no`      | Ne participera pas                             |
| `maybe`   | Participation incertaine                       |

  

## 🔒 Sécurité

  

-  **Mots de passe hashés** avec BCrypt (salt rounds: 12)

-  **Tokens JWT signés** avec HMAC SHA-256

-  **Validation des entrées** avec Data Annotations

-  **CORS configuré** pour le développement

  

## 🧪 Tests

  

Pour tester l'API :

 **Swagger UI** : Interface interactive à `http://localhost:5021/swagger`


  

## 📝 Exemples d'utilisation

  

### Inscription

```bash

curl  -X  POST  "http://localhost:5021/api/auth/signup"  \

-H "Content-Type: application/json" \

-d  '{

"name": "Jean Dupont",

"email": "jean@example.com",

"password": "motdepasse123"

}'

```

  

### Créer un événement

```bash

curl  -X  POST  "http://localhost:5021/api/events"  \

-H "Authorization: Bearer {votre-token}" \

-H  "Content-Type: application/json"  \

-d '{

"title":  "Mon événement",

"description":  "Description de l événement",

"date":  "2024-12-25T15:00:00Z",

"location":  "Paris",

"maxParticipants":  50,

"isPublic":  true

}'

```
