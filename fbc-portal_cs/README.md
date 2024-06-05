# pontual-portal-html-2.0

This project was generated with [Angular CLI](https://github.com/angular/angular-cli) version 8.0.1

## Prerequisite Softwares
Min 2 GB free space on the drive
Node Js - Version 12+
Git 2+
npm install -g typescript
npm install -g @angular/cli

## Development server

Run `npm install` to install dependencies
Run `ng serve` for a dev server. Navigate to `http://localhost:4200/`. The app will automatically reload if you change any of the source files.

## Code scaffolding

Run `ng generate component component-name` to generate a new component. You can also use `ng generate directive|pipe|service|class|guard|interface|enum|module`.

## Build

Run `ng build` to build the project. The build artifacts will be stored in the `dist/` directory. Use the `--prod` flag for a production build.

## Running unit tests

Run `ng test` to execute the unit tests via [Karma](https://karma-runner.github.io).

## Running end-to-end tests

Run `ng e2e` to execute the end-to-end tests via [Protractor](http://www.protractortest.org/).

## Further help

To get more help on the Angular CLI use `ng help` or go check out the [Angular CLI README](https://github.com/angular/angular-cli/blob/master/README.md).

## Further help

To get more help on the Angular CLI use `ng help` or go check out the [Angular CLI README](https://github.com/angular/angular-cli/blob/master/README.md).


## Firebase Deploy -- Only for production
node --max_old_space_size=8192 node_modules/@angular/cli/bin/ng build --prod --output-hashing=all
ng build --configuration production
firebase projects:list
firebase use <instance name>
firebase login
firebase deploy

## Types of Layouts

Vertical Menu Template
Semi Dark Vertical Menu Template
Dark Nav Vertical Menu Template
Light Vertical Menu Template

Vertical Modern Menu Template
Vertical Overlay Menu Template
Horizontal Menu Template
Full Width Horizontal Menu Template

## Create New Module and Module

# Add Module
ng g m components
cd .\src\app\components\

# Add component
ng g c <componentname>
add new menu in menu setting
add new route in module and refernce to new <componentname>
copy html to component html

# Convert from TSLint to ESLint
ng add @angular-eslint/schematics
ng g @angular-eslint/schematics:convert-tslint-to-eslint

## Flickering issue
Remove all occurences of below text from index.html generated in dist folder
media="print" onload="this.media='all'"


