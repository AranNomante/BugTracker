// Wait for the DOM to be ready

$(function () {  
    // Initialize form validation on the registration form.
    // It has the name attribute "create"
    $("form[name='create']").validate({
        // Specify validation rules
        rules: {
            // The key name on the left side is the name attribute
            // of an input field. Validation rules are defined
            // on the right side
            title: {
                required: true,
                maxlength: 255
            },
            description: {
                required: true,
                maxlength: 4000
            },
            version: {
                required: true,
                maxlength: 45
            }
        },
        // Specify validation error messages
        messages: {
            title: {
                required: "Please enter a title",
                maxlength: "Title is too long"
            },
            description: {
                required: "Please enter a description",
                maxlength: "Description is too long"
            },
            version: {
                required: "Please enter a version",
                maxlength: "Version too long"
            }
        },
        // Make sure the form is submitted to the destination defined
        // in the "action" attribute of the form when valid
        submitHandler: function (form) {
            form.submit();
        }
    });
    $("form[name='edit']").validate({
        // Specify validation rules
        rules: {
            // The key name on the left side is the name attribute
            // of an input field. Validation rules are defined
            // on the right side
            title: {
                required: true,
                maxlength: 255
            },
            description: {
                required: true,
                maxlength: 4000
            },
            version: {
                required: true,
                maxlength: 45
            },
            fdescription: {
                maxlength:4000
            }
        },
        // Specify validation error messages
        messages: {
            title: {
                required: "Please enter a title",
                maxlength: "Title is too long"
            },
            description: {
                required: "Please enter a description",
                maxlength: "Description is too long"
            },
            version: {
                required: "Please enter a version",
                maxlength: "Version too long"
            },
            fdescription: "Fix Description is too long" 
        },
        // Make sure the form is submitted to the destination defined
        // in the "action" attribute of the form when valid
        submitHandler: function (form) {
            form.submit();
        }
    });
    $("form[name='asedit']").validate({
        // Specify validation rules
        rules: {
            // The key name on the left side is the name attribute
            // of an input field. Validation rules are defined
            // on the right side
            password: {
                required: true,
                maxlength: 10
            }
        },
        // Specify validation error messages
        password: {
            required: "Please enter a password",
            maxlength: "Password too long"
        },
        // Make sure the form is submitted to the destination defined
        // in the "action" attribute of the form when valid
        submitHandler: function (form) {
            form.submit();
        }
    });
    $("form[name='ascreate']").validate({
        // Specify validation rules
        rules: {
            // The key name on the left side is the name attribute
            // of an input field. Validation rules are defined
            // on the right side
            email: {
                required: true,
                email: true,
                maxlength:254
            },
            password: {
                required: true,
                maxlength: 10
            }
        },
        // Specify validation error messages
        messages: {
            email: {
                required: "Please enter an email",
                email: "Please enter a valid email",
                maxlength: "Email too long"
            },
            password: {
                required: "Please enter a password",
                maxlength: "Password too long"
            }
        },
        // Make sure the form is submitted to the destination defined
        // in the "action" attribute of the form when valid
        submitHandler: function (form) {
            form.submit();
        }
    });
    $("form[name='usedit']").validate({
        // Specify validation rules
        rules: {
            // The key name on the left side is the name attribute
            // of an input field. Validation rules are defined
            // on the right side
            password: {
                required: true,
                maxlength: 10
            }
        },
        // Specify validation error messages
        password: {
            required: "Please enter a password",
            maxlength: "Password too long"
        },
        // Make sure the form is submitted to the destination defined
        // in the "action" attribute of the form when valid
        submitHandler: function (form) {
            form.submit();
        }
    });
    $("form[name='uscreate']").validate({
        // Specify validation rules
        rules: {
            // The key name on the left side is the name attribute
            // of an input field. Validation rules are defined
            // on the right side
            email: {
                required: true,
                email: true,
                maxlength: 254
            },
            password: {
                required: true,
                maxlength: 10
            }
        },
        // Specify validation error messages
        messages: {
            email: {
                required: "Please enter an email",
                email: "Please enter a valid email",
                maxlength: "Email too long"
            },
            password: {
                required: "Please enter a password",
                maxlength: "Password too long"
            }
        },
        // Make sure the form is submitted to the destination defined
        // in the "action" attribute of the form when valid
        submitHandler: function (form) {
            form.submit();
        }
    });
    $("form[name='admedit']").validate({
        // Specify validation rules
        rules: {
            // The key name on the left side is the name attribute
            // of an input field. Validation rules are defined
            // on the right side
            password: {
                required: true,
                maxlength: 10
            }
        },
        // Specify validation error messages
        password: {
            required: "Please enter a password",
            maxlength: "Password too long"
        },
        // Make sure the form is submitted to the destination defined
        // in the "action" attribute of the form when valid
        submitHandler: function (form) {
            form.submit();
        }
    });
    $("form[name='admcreate']").validate({
        // Specify validation rules
        rules: {
            // The key name on the left side is the name attribute
            // of an input field. Validation rules are defined
            // on the right side
            email: {
                required: true,
                email: true,
                maxlength: 254
            },
            password: {
                required: true,
                maxlength: 10
            }
        },
        // Specify validation error messages
        messages: {
            email: {
                required: "Please enter an email",
                email: "Please enter a valid email",
                maxlength: "Email too long"
            },
            password: {
                required: "Please enter a password",
                maxlength: "Password too long"
            }
        },
        // Make sure the form is submitted to the destination defined
        // in the "action" attribute of the form when valid
        submitHandler: function (form) {
            form.submit();
        }
    });
    $("form[name='login']").validate({
        // Specify validation rules
        rules: {
            // The key name on the left side is the name attribute
            // of an input field. Validation rules are defined
            // on the right side
            Email: {
                required: true,
                email: true,
                maxlength: 254
            },
            Password: {
                required: true,
                maxlength: 10
            }
        },
        // Specify validation error messages
        messages: {
            Email: {
                required: "Please enter an email",
                email: "Please enter a valid email",
                maxlength: "Email too long"
            },
            Password: {
                required: "Please enter a password",
                maxlength: "Password too long"
            }
        },
        // Make sure the form is submitted to the destination defined
        // in the "action" attribute of the form when valid
        submitHandler: function (form) {
            form.submit();
        }
    });
    $("form[name='editman']").validate({
        // Specify validation rules
        rules: {
            // The key name on the left side is the name attribute
            // of an input field. Validation rules are defined
            // on the right side
            mantext: {
                maxlength: 4000
            }
        },
        // Specify validation error messages
        messages: {
            mantext: {
                maxlength: "Manual is too long"
            }
        },
        // Make sure the form is submitted to the destination defined
        // in the "action" attribute of the form when valid
        submitHandler: function (form) {
            form.submit();
        }
    });
});