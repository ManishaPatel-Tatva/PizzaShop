function togglePassword(inputId, icon) {
    var inputField = document.getElementById(inputId);
    if (inputField.type === "password") {
        inputField.type = "text";
        icon.classList.remove("fa-eye-slash");
        icon.classList.add("fa-eye");
    } else {
        inputField.type = "password";
        icon.classList.remove("fa-eye");
        icon.classList.add("fa-eye-slash");
    }
}

//toggle sidebar
$(document).on("click","#toggleSidebarBtn",function(){
    var sidebar = $("#sidebar");
    if(sidebar.css("display") == "none")
    {
        sidebar.css("display", "block");
    }
    else
    {
        sidebar.css("display", "none");
    }
})

