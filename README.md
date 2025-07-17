# 📅 Event Planner

> **Application de gestion d'événements avec système d'invitations privées**

[![.NET](https://img.shields.io/badge/.NET-8.0-blue)](https://dotnet.microsoft.com/)
[![Alpine.js](https://img.shields.io/badge/Alpine.js-3.x-green)](https://alpinejs.dev/)
[![SQLite](https://img.shields.io/badge/SQLite-Database-lightgrey)](https://sqlite.org/)

## 🎯 Description

Application complète avec **API ASP.NET Core 8** et **frontend Alpine.js** pour gérer des événements publics ou privés avec invitations par email.

## ✨ Fonctionnalités

- 🔐 **Authentification JWT** sécurisée
- 📅 **Événements publics** accessibles à tous
- 🔒 **Événements privés** avec invitations par email
- 📱 **Interface responsive** moderne
- 👥 **Gestion des participants** et RSVP

## 🚀 Démarrage rapide

```bash
# Backend
cd EventPlannerAPI
dotnet restore && dotnet ef database update && dotnet run

# Frontend
cd EventPlannerFront
python -m http.server 3000
```

**URLs** : API `http://localhost:5021` • Frontend `http://localhost:3000` • Swagger `http://localhost:5021/swagger`

## 📖 Documentation complète

### 🔗 **[📚 Consultez le Wiki GitHub](https://github.com/willybeller/event_planner/wiki)**

Toute la documentation détaillée est disponible dans le wiki :

| Page | Description |
|------|-------------|
| **[🚀 Getting Started](https://github.com/willybeller/event_planner/wiki/Getting-Started)** | Installation et configuration |
| **[🔧 API Documentation](https://github.com/willybeller/event_planner/wiki/API-Documentation)** | Endpoints et modèles |
| **[🎨 Frontend Guide](https://github.com/willybeller/event_planner/wiki/Frontend-Documentation)** | Interface utilisateur |

## 🛠️ Stack

**Backend** : ASP.NET Core 8, Entity Framework, SQLite, JWT  
**Frontend** : Alpine.js, Tailwind CSS, Font Awesome
