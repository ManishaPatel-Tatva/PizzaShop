// Function to reinitialize validation for dynamically added elements
function reinitializeValidation() {
    $("form").each(function () {
      $.validator.unobtrusive.parse($(this));
    });
  }
  
  // Call this function after any AJAX request that adds forms dynamically
  $(document).ajaxComplete(function () {
    reinitializeValidation();
  });
  
  // Apply validation on input change globally
  $(document).on("keyup change", "form input:not([type=checkbox]):not([type=radio]),form select,form textarea", function () {
    $(this).valid();
  });

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


// $(document).ready(function () {
//     var categoryId = $('#category_listUl li:first>div>a').data("id");
//     $('#category_listUl li:first>div>a').addClass("category_active");
//     paginationAjax(1);
//   });

//   $('.list-items>div>a').on("click", function () {
//     $('.list-items').each(function () {
//       $('.list-items>div>a').removeClass("category_active");
//     });
//     categoryId = $(this).data("id");
//     $(this).addClass("category_active");
//     paginationAjax(1);
//   })