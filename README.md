# Projet 8 : Améliorez votre application avec des systèmes distribués

**Date de création** : 01 mars 2026

**Date de la dernière modification** : 01 mars 2026

**Auteur** : Alban VOIRIOT

**Informations techniques** :

- **Technologies** : C#, .NET, Xunit
- **Version de .NET ** : 8.0

## Sommaire

- [Contexte](#contexte)
- [Installation](#installation)
  - [Télécharger le projet](#télécharger-le-projet)
- [Exécuter les tests](#exécuter-les-tests)

## Contexte

Ce projet a été conçu dans le cadre de ma formation de développeur d'applications Back-end .NET (OpenClassrooms) sur l'amélioration sur les performances d'une application, corrections de tests unitaire et CI automatique avec GitHub Actions


## Installation

### Télécharger le projet

=> Pour télécharger le dossier, veuillez effectuer la commande GIT ou directement dans le GIT de Visual Studio : `git clone https://github.com/Alban2001/Projet8NETTourGuide.git` dans un dossier vide puis ouvrir la solution du projet.

### Restaurer les dépendances

=> Veuillez effectuer la commande : `dotnet restore` afin de pouvoir avoir tous les packages Nuget et composants installés pour éviter toute erreur de librairie manquante.

## Exécuter les tests

=> Allez dans l'onglet à droite (Explorateur de tests). Si cette partie n'est pas encore affichée, allez dans l'onglet "Test" en haut puis "Explorateur de tests".

=> Vous pouvez soit éxécuter tous les tests ou bien exécuter un test en particulier à chaque fois.

=> Dans les performances de tests, vous apercevrez ceci [Fact(Skip = "Temporairement désactivé en CI")] sur le dessus des fonctions. Afin de les lancer, vous devez retirer les Skip sinon les tests seront ignorés. 

N'hésitez pas aussi à changer la valeur dans la procédure _fixture.Initialize(X) pour exécuter les performances avec un nombre d'utilisateurs.

Pour rappel, le but de se projet a été d'améliorer les performances de l'application (trop lente) et prévoir une prévision de 100 000 utilisateurs par jour.
