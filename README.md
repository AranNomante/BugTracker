# BugTracker
Internship Project; A Bug Filing Management System

Current Overview of The Project:
    
    Useful Materials:
      
      https://www.w3schools.com/css/css3_object-fit.asp

      https://getbootstrap.com/docs/4.4/

      https://www.w3schools.com/bootstrap4/bootstrap_buttons.asp

     
Latest Project Information (To be Updated):

    Definition: 
        •	Bug Tracking Software
    Features:
        •	User Authentication (security etc.)
        •	Cookie Auth. 
        •	Filing Bugs (Brief explanation on how to reproduce the problem, what the problem is, attachments such as screenshots etc.)
        •	Viewing bug status’ (severity, OS, solved/active, assignee etc.)
        •	Updating bug status’(adding info, deleting, marking solved etc.)
        •	(DBMS, MVC, Razor, C#, Jquery, CSS, HTML)
        •	Master Admin (master@....)

    Db:
        For simplicity and server-side performance database entities will have minimal logic,
        Most of the constraints will be checked from client-side.
    Feature List:
        User Authentication;
	        Cookie based 
        Admin Panel;
	        For adding/removing users, assignees, removing filed bugs. 
        Assignee Panel;
	        For viewing assigned active/closed bugs
        Statistics;
	        Active bugs, closed bugs etc.
        Bug filing;
            Search tickets, open ticket, view ticket, edit ticket, mark ticket, add comment.
        Admin scope: Search, view, edit, mark
        
        Assignee scope: Search, view, mark, add comment
        
        User scope: search, view, open      

        General Pages:
	        Home 
	        Bugs (details will be dynamic; admin will see assignee and the user who posted it)
        	Login
	        Logout (not page)
        	Error page
        Special Pages:
            Admin:
		        Admin panel
			    View Users
			    View Assignees
	        Assignee:
		        Assignee panel
	    Cookie-Based Authentication
            Cookie-based authentication has been the default, tried-and-true method for handling user authentication for a long time.
            Cookie-based authentication is stateful. This means that an authentication record or session must be kept both server and client-side. The server needs to keep track of active sessions in a database, while on the front-end a cookie is created that holds a session identifier, thus the name cookie based authentication. Let's look at the flow of traditional cookie-based authentication:
            1.	User enters their login credentials.
            2.	Server verifies the credentials are correct and creates a session which is then stored in a database.
            3.	A cookie with the session ID is placed in the users browser.
            4.	On subsequent requests, the session ID is verified against the database and if valid the request processed.
            5.	Once a user logs out of the app, the session is destroyed both client-side and server-side.



