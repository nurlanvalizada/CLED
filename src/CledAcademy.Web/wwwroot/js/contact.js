$("#contact_form, #quick_contact_form2")
    .on("submit",
        function(e) {
            e.preventDefault();
            var form = this;
            if ($(form).valid()) {
                var formBtn = $(this).find('button[type="submit"]');
                var formResultDiv = '#form-result';
                $(formResultDiv).remove();
                formBtn
                    .before('<div id="form-result" class="alert alert-success" role="alert" style="display: none;"></div>');
                var formBtnOldMsg = formBtn.html();
                formBtn.html(formBtn.prop('disabled', true).data("loading-text"));
                $(form)
                    .ajaxSubmit({
                        type: "POST",
                        url: "/Home/Contact",
                        jsonData: $("#contact_form").serialize(),
                        success: function(data) {
                            if (data.result == true) {
                                $(form).find('.form-control').val('');
                            }
                            formBtn.prop('disabled', false).html(formBtnOldMsg);
                            $(formResultDiv).html(data.message).fadeIn('slow');
                            setTimeout(function() { $(formResultDiv).fadeOut('slow') }, 4000);
                        }
                    });
            }
        });