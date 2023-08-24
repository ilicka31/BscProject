# **DIPLOMSKI**
**Migracija monolitnog sistema na mikroservisnu arhitekturu baziranu na dogadjajima**

***Zadatak: Izvršiti pregled literature, identifikovati moguće podele postojećih servisa na mikroservisne koristeći metode dostupne u literaturi. Implementirati podelu. Upotrebiti event sourcing šablon kako bi se implementirala arhitektura bazirana na događajima i predstaviti par slučajeva gde ona donosi prednosti.***

Pre pokretanja aplikacije, neophodno je instalirati softver sa sledeceg linka: https://www.docker.com/ 
Nakon instalacije, otvoriti cmd, pozicionirati se u folder u kome se nalazi yaml fajl. Izvrsiti naredbu 'docker-compose up'.

Proveriti da li je pokrenut RabbitMQ servis, inicijalni kredencijali su guest/guest -> http://localhost:15672/#/

Solution aplikacije se nalazi u folderu WebMicroservices, neohodno je pokrenuti solution u *Multiple startup projects* modu, redosled aplikacija je sledeci:
  1. UserService : Start without debbuing
  2. ProductService : Start without debbuing
  3. OrderService : Start without debbuing
  4. APIGateway : Start 

Nakon uspesnog pokretanja solution-a, potrebno je kreirati admina aplikacije, preko aplikacije *Postman* :
  POST :
  http://localhost:5005/register
    {
        "name": "admin",
        "username": "admin",
        "email": "admin@admin.com",
        "password": "string",
        "birthDate": "2000-07-31T18:00:00.000Z",
        "address": "address",
        "type": 2
    }

Fronted aplikacije nalazi se u folderu *Front*. Potrebno je ovaj folder otvoriti u Visual Studio Code-u. Zatim izvrsiti naredbu 'npm install', kako bi se instalirali svi neophodni moduli. Nakon instalacije, aplikacija se pokrece komandom 'npm start'.

Aplikacija se pokrece na http://localhost:3000, predlozeni pregledac za pokretanje je Google Chrome.
