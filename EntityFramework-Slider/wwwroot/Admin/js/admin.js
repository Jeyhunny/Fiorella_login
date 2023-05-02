

$(function () {

   



    $(document).on("click", ".slider-status", function () {

        let changeElement = $(this);

        let sliderId = $(this).parent().attr("data-id");

        let data = { id: sliderId }

        $.ajax({
            url: "slider/setstatus",   
            type: "Post",
            data: data,                
            success: function (res) {

                if (res) {
                    $(changeElement).removeClass("active-status");
                    $(changeElement).addClass("deActive-status");
                }
                else
                {
                    $(changeElement).addClass("active-status");
                    $(changeElement).removeClass("deActive-status");
                }
               


            }
        })
    })




    $(document).on("submit", "#category-delete-form", function (e) {   

        e.preventDefault();

        let categoryID = $(this).attr("data-id");

        let deletedElem = $(this);

        let data = { id: categoryID };


        $.ajax({
            url: "category/SoftDelete",    
            type: "post",
            data: data,                 
            success: function (res) {

                $(deletedElem).parent().parent().remove();
            }
        })
    })


})


