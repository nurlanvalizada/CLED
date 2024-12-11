$(".product-remove").on("click",
    function () {
        var orderId = $(this).children("input[type=hidden][class=orderId]").val();
        var jsonString = "{" + '"id":' + orderId + "}";
        var jsonData = window.jQuery.parseJSON(jsonString);
        $.ajax({
            url: '/api/MyApi/DeleteItem',
            type: 'POST',
            data: jsonData,
            dataType: 'json',
            success: function () {
                $('.product-remove>input[type=hidden][value="' + orderId + '"]').parent().parent().remove();
                var prices = $(".item_price_holder>.product-price>.amount").text();
                var totalPrice = 0;
                for (var i = 0; i < prices.length; i++) {
                    totalPrice += parseInt(prices[i]);
                }
                $("#orderCount").text(prices.length);
                $(".totalAmount").text(totalPrice);
                if (prices.length === 0) {
                    $("#viewCard").addClass("disabled");
                    $(".checkout").addClass("disabled");
                }
            }
        });
    });