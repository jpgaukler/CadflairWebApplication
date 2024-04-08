# Cadflair

## What is Cadflair?
Cadflair is an digital product catalog for manufacturing companies, that makes it simple for companies to share CAD data on the web. Simply create 3D models with a parametric CAD tool (preferably Autodesk Inventor) and publish them to the Cadflair site. Use the online portal to organize your models into a catalog that showcases what you have to offer to your customers. Share links to your models so your customers can view your products with a single click!

## Why did I build this?
My programming journey began with writing VB.NET scripts to automate tasks in Autodesk Inventor (a CAD modeling software for mechanical engineers). In doing so, I stumblied across [Autodesk Platform Services](https://aps.autodesk.com/) (formerly Autodesk Forge), which provides developers with a set of web APIs to leverage the power of Autodesk software inside web applications. I created an account and started making API calls using Postman, having no idea what I was doing... but after working through a tutorial, I was hooked. 

As a mechanical engineer, I spent a lot of time working with CAD data, but noticed that communication often broke down when external stakeholders did not have access to the 3D models that I was working with. Sure, there are ways to share CAD files, but you often need specialized software just to view the models. What if anyone could just click a link and view a 3D model on their desktop or mobile device? **With the Viewer available through Autodesk Platform Services, you can do just that!**

My goal was to build a website that could serve as an online catalog showcasing simple 3D CAD models. Without any web development experience, I thought this would be a fun way to learn how web development worked, and create something that I thought would be useful. I imagined that I was building a startup SAAS product that could be used by small to mid-sized manufacturing companies.

## How does it work?
Check out the diagram below for a simple overview of the application architecture! In short, the site consists of a React app on the front-end, with is supported by a REST API and SQL database on the backend. I used Autodesk Platform Services (formerly Autodesk Forge) for processing and storing the CAD files. 

![Cadflair Diagram](https://github.com/jpgaukler/CadflairWebApplication/assets/97172053/375e51d7-38cc-4233-b65a-4baf5a0def76)


## What did I learn?
I LEARNED A LOT!

### Front-end 
* Internet protocols (HTTP/HTTPS)
* Web-sockets (Blazor Server) vs REST APIs
* Difference between client and server
* Front-end UI frameworks (React)
* Typescript for type-checking in Javascript (feels kinda like C#!)
* Token based authentication (for securing site and accessing third-part APIs)
* Client vs server side rendering strategies
* Building reusable UI components
* Navigation strategies

### Back-end
* Writing SQL queries and designing table schemas
* Calling stored procedures with parameterized queries
* Using Azure SQL to persist data (Microsoft SQL Server)
* Seperating business logic from front-end
* Server side data validation
* How to consume third-party apis (Autodesk Platform Services)
* Testing REST APIs with Postman
* Finding errors in production with Azure Application Insights
* Storing static files (images) in cloud storage (Azure Blob)

### Hosting and Configuration
* Setting up a domain name
* Configuring DNS records
* Deploying ASP.NET Core applications to Azure App Services
* Pricing models in Microsoft Azure
* Managing cloud resources and billing
* Deploying code to production and staging slots

### Other Skills
* Source control using Github
* Running code in local environment vs production enviroment (not always the same!)
* Managing project dependencies with NuGet
* Logging and debugging


## Results
Cadflair changed as my skills and knowledge of web development grew. Although I have not completed every feature that I first imagined, I was successful in creating a demo that showcases what Cadflair could become. I set up a simple catalog that includes various pipe and flange products. (I chose pipe fittings because the CAD models were simple and easy to make). Initially I only knew C#, so I created the site entirely in the .NET stack, and hosted a Blazor Server app to act as both front-end and back-end. I later wrote a REST API so I could experiment with building the front end using React and Angular (I really enjoy Angular, it feels very similar to Blazor!) Take a look at the different approaches using the links below!

### Compare the resuts here!
* Version 1: [Cadflair - Blazor Server](https://cadflair.azurewebsites.net/)
* Version 2: [Cadflair - React](https://react.cadflair.com/)
* IN PROGRESS! - Version 3: [Cadflair - Angular](https://angular.cadflair.com/)


