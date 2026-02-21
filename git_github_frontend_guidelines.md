# Guide de coordination Git & GitHub -- Équipe Frontend (.NET Niger)

Ce document explique comment l'équipe frontend doit utiliser Git et
GitHub pour travailler ensemble efficacement.


------------------------------------------------------------------------

## 🎯 Objectifs

-   Travailler à plusieurs sans écraser le travail des autres\
-   Garder un projet organisé\
-   Pouvoir intégrer facilement les nouvelles fonctionnalités

------------------------------------------------------------------------

## Cloner le projet depuis github

-   Entre dans ton dossier de travail
-   tu ouvres le terminal git(bash)
-   git clone https://github.com/AbdoulRaouf2005/DotnetNiger-Frontend.git
-   cd DotnetNiger-Frontend
- dotnet restore

------------------------------------------------------------------------

# iniatialisation de tailwindCss 

-   assurez-vous d'avoir installer nodejs (vous pouvez vérifier en tapant node --v)
-   sur les lignes de commandes allez-y à la source du projet 
-   tapez npm install
-   executez npx tailwindcss -i ./wwwroot/css/input.css -o ./wwwroot/css/output.css --watch
-   ou executer le fichier "Tailwind Watch.sh"

Règles : -assurez-vous de toujours executer la derniere commande avant de commencer une implementation


------------------------------------------------------------------------

## 🏗️ Structure des branches

    develop     → branche par défaut (version d’intégration)
    master      → version stable
    feature/... → branches de travail

Règles :
- Ne jamais pousser directement sur **master** (réservé aux releases stables)
- Ne jamais pousser directement sur **develop** sans passer par une Pull Request
- Tout le monde crée ses branches depuis **develop**
- La branche par défaut du dépôt GitHub est **develop**

------------------------------------------------------------------------

## 🌱 Création des branches

Exemples :

    feature/home-page
    feature/navbar
    feature/footer
    feature/events-page
    feature/blog-page

------------------------------------------------------------------------

## 🔁 Cycle de travail standard

### 1. Cloner le projet

    git clone <lien_du_repo>

### 2. Aller sur develop

    git switch develop

### 3. Créer sa branche

    git branch feature/ma-fonctionnalite

### 4. Travailler

Coder normalement

### 5. Sauvegarder

    git add .
    git commit -m "Add home page layout"
    git push origin feature/ma-fonctionnalite

### 6. Créer une Pull Request

Sur GitHub : - Source : feature/ma-fonctionnalite - Destination :
develop

------------------------------------------------------------------------

## 🔄 Mettre à jour son projet local avec les nouveautés

Quand un autre membre ajoute des fonctionnalités sur develop :

### Étape 1 : Aller sur la branche de developpement( branch develop)

    git switch develop

### Étape 2 : Télécharger les nouveautés

    git pull origin develop

### Etape 3 : Retourne sur ta branche de developpement de la fonctionnalité (feature)

    git switch feature/...

### Etape 4 : Travailler

    coder normalement ou tester la nouvelle fonctionnaté


------------------------------------------------------------------------

## ⚠️ En cas de conflit

Dans le fichier :

    <<<<<<< HEAD
    ton code
    =======
    leur code
    >>>>>>> develop

Corriger puis :

    git add .
    git commit -m "Resolve merge conflict"

------------------------------------------------------------------------

## 🔁 Mise à jour quotidienne recommandée

Chaque jour :

    git switch develop
    git pull origin develop
    git switch feature/ma-fonctionnalite
    git merge develop

------------------------------------------------------------------------

## ❌ À ne jamais faire

    git push origin master
    git pull origin master


------------------------------------------------------------------------

## 🧭 Résumé rapide

    1. Créer branche
    2. Coder
    3. Commit
    4. Push
    5. Pull Request

------------------------------------------------------------------------

## Le pull request se fait uniquement par une seul personne sur github

### Structure du projet Blazor à suivre
```
dotnetniger-frontend/
├── Pages/
│   ├── Home.razor
│   ├── Community.razor
│   ├── Events.razor
│   ├── Partners.razor
│   ├── Contact.razor
│   ├── Blog.razor
│   ├── About.razor
│   ├── Resources.razor
│   ├── Admin/
│   │   ├── Dashboard.razor
│   │   ├── Users.razor
│   │   └── ...
├── Components/
│   ├── Shared/
│   │   ├── Topbar.razor
│   │   ├── Sidebar.razor
│   │   └── Footer.razor
│   ├── Cards/
│   ├── Forms/
│   └── Admin/
├── Services/
│   └── ApiService.cs
├── Models/
├── wwwroot/
│   ├── index.html (+ Bootstrap CDN)
│   └── css/
│       ├── header-overrides.css
│       └── admin.css
└── Program.cs

```


Fin du document
