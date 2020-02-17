# BugTracker
Internship Project; A Bug Filing Management System

Visual Studio 2019 & Microsoft SQL Server 2017 & Microsoft SQL Server Management Studio 2019 were used in development session.

SQL Server Management Studio may not be needed since visual studio offers inbuilt database management functions

Check out the link below for creating a database from Model1.edmx(Be aware of the content in connectionStrings tag at "BugTracker/Web.config"; you may need to erase the existing connection string before configuring and adding a new database from .edmx file) :

https://stackoverflow.com/questions/7555388/how-can-i-generate-the-database-from-edmx-file-in-entity-framework

See packages.config if visual studio does not import nuget packages automatically

There may be redundant dependencies, code, files in project since the development time was limited and this project's aim was to research and study.

Summary:
Bug filing & management system for general purpose use
Project Features:
Bug filing
User Authentication using Cookies
User Specific Content and Authorizations
Bug Status Updates
Viewing Bug Details
User Types
Admin Panel
Assignee Panel
Statistics
Record Search and Sort
Technologies:
DBMS
ASP.NET MVC 5
JQuery
Javascript
HTML
CSS
Bootstrap
Encryption
DBMS:
Microsoft SQL Server

Encryption:
Cookie encryption(two layer) using SHA256 and Rail Fence Cipher
Bug Flow:
1-User creates a bug report
2-Admin views the report and assigns it to an assignee
3-Assignee views the assigned report and submits a fix report
