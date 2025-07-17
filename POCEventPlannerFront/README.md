
# 🎨 POC Event Planner Frontend

  

Interface web moderne pour la gestion d'événements, développée avec Alpine.js et Tailwind CSS.

  

## 🚀 Fonctionnalités

  

### 🔐 Authentification utilisateur

-  **Formulaires** d'inscription et connexion responsives

-  **Gestion automatique** des tokens JWT

-  **Redirection** intelligente selon l'état d'authentification

-  **Déconnexion** avec nettoyage de session

  

### 📅 Interface d'événements

-  **Création d'événements** avec modal interactive

-  **Système d'événements privés** avec gestion d'invitations

-  **Filtrage dynamique** (tous, mes événements, rejoints, privés, publics)

-  **Recherche en temps réel** par titre

-  **Affichage responsive** en grille adaptative

  

### 👥 Gestion des participants

-  **Boutons d'action** contextuels (Rejoindre/Organisateur/Participant)

-  **Modal de détails** complète avec informations participants

-  **Indicateurs visuels** pour les statuts d'événements

-  **Liste des invités** pour les événements privés

  

### 🎨 Design et UX

-  **Interface responsive** mobile-first

-  **Modales adaptatives** avec gestion du scroll

-  **Animations fluides** avec Alpine.js

-  **Système d'alertes** pour les notifications

-  **Indicateurs de statut** colorés et intuitifs

  

## 🛠️ Technologies utilisées

  

-  **Alpine.js 3.x** - Framework JavaScript réactif et léger

-  **Tailwind CSS 2.2** - Framework CSS utilitaire

-  **Font Awesome 6.4** - Bibliothèque d'icônes vectorielles

-  **Fetch API** - Communication HTTP avec l'API

-  **CSS Grid/Flexbox** - Layout responsive moderne

  

## 📋 Prérequis

  

- Un navigateur web moderne (Chrome, Firefox, Safari, Edge)

- Serveur web local (optionnel mais recommandé)

- API EventPlanner en cours d'exécution sur `http://localhost:5021`

  

## 🔧 Installation et démarrage

  

### 1. Fichiers statiques

```bash

# Cloner le repository (si pas déjà fait)

git  clone  https://github.com/willybeller/event_planner.git

cd  event_planner/POCEventPlannerFront

```

  

### 2. Méthode 1: Ouverture directe

```bash

# Ouvrir index.html dans un navigateur

open  index.html

# ou double-clic sur le fichier

```

  

### 3. Méthode 2: Serveur local

```bash

# Avec Python 3

python  -m  http.server  3000

  

# Avec Node.js (si live-server installé)

npx  live-server  --port=3000

  

# Avec VS Code Live Server extension

# Clic droit sur index.html > "Open with Live Server"

```

  

### 4. Accès à l'application

-  **Frontend** : `http://localhost:3000`

-  **API Backend** : `http://localhost:5021` (doit être démarré)

  

## 📁 Structure du projet

  

```

EventPlannerFront/

├── index.html # Page principale de l'application

├── app.js # Logique Alpine.js et communication API

├── styles.css # Styles CSS personnalisés

└── README.md # Documentation frontend

```

  

## 🎯 Guide d'utilisation

  

### Première connexion

1.  **Ouvrir** l'application dans le navigateur

2.  **S'inscrire** avec un nouveau compte

3.  **Se connecter** avec les identifiants créés

4.  **Commencer** à créer des événements

  

### Créer un événement public

1.  **Cliquer** sur "Créer un événement"

2.  **Remplir** le formulaire (titre, description, date, heure)

3.  **Laisser** "Événement privé" décoché

4.  **Publier** l'événement

  

### Créer un événement privé

1.  **Cliquer** sur "Créer un événement"

2.  **Remplir** les informations de base

3.  **Cocher** "Événement privé"

4.  **Ajouter** les emails des invités un par un

5.  **Publier** l'événement avec invitations

  

### Navigation et filtres

-  **Tous les événements** : Voir tous les événements accessibles

-  **Mes événements** : Événements créés par l'utilisateur

-  **Événements rejoints** : Événements où l'utilisateur participe

-  **Événements privés** : Uniquement les événements privés accessibles

-  **Événements publics** : Uniquement les événements publics

  

## 🎨 Interface utilisateur

  

### Composants principaux

-  **Barre de navigation** : Authentification et profil utilisateur

-  **Filtres et recherche** : Contrôles pour affiner l'affichage

-  **Grille d'événements** : Affichage responsive des événements

-  **Modales** : Création, édition et détails des événements

  

### Indicateurs visuels

| Élément | Description | Couleur |

|---------|-------------|---------|

| 🔒 Badge "Privé" | Événement sur invitation | Violet |

| 👤 Badge "Organisateur" | Créé par l'utilisateur | Bleu |

| ✅ Badge "Participant" | Utilisateur a rejoint | Vert |

| ⏰ Badge "Terminé" | Date passée | Gris |

  

### Actions contextuelles

-  **Organisateur** : Modifier, Supprimer, Voir détails

-  **Non-participant** : Rejoindre (si autorisé), Voir détails

-  **Participant** : Voir détails

-  **Événement privé** : Accessible seulement aux invités

  

## ⚙️ Configuration

  

### API Backend

```javascript

// Dans app.js - Configuration de l'URL de l'API

API_BASE_URL: 'http://localhost:5021/api'

``` 
 

### Communication avec l'API

-  **Headers JWT** : Ajout automatique du token d'authentification

-  **Gestion d'erreurs** : Affichage d'alertes pour les erreurs

-  **Rechargement automatique** : Mise à jour des données après actions