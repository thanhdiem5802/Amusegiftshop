function updateCartContent() {
    $.ajax({
        url: '/Cart/GetCart',
        type: 'GET',
        success: function (data) {
            var cartItemsHtml = '';
            var totalPrice = data.totalPrice.toFixed(0); // Làm tròn đến 0 chữ số sau dấu thập phân

            if (data.cartModels && data.cartModels.length > 0) {
                data.cartModels.forEach(function (item) {
                    console.log(item);
                    cartItemsHtml += '<div class="cart-bar__item position-relative d-flex flex-column mb-3">' +
                        '<div class="thumb mb-2">' +
                        '<img src="' + item.productModel.url + '" alt="image_not_found">' +
                        '</div>' +
                        '<div class="content">' +
                        '<h4 class="title mb-2" >' +
                        '<a href="' + item.productModel.url + '">' + item.productModel.name + '</a>' +
                        '</h4>' +
                        '<span class="Giá">' + ((item.productModel.price * item.quantity) - (item.productModel.price * item.quantity * item.percentage / 100)).toFixed(0) + ' đ</span>' +
                        '<a href="javascript:void(0)" class="product-remove" onclick="removeFromCart(\'' + item.productModel.productId + '\')" style="top: -68%; right: 60px; color: #ffffff; width: 34px; height: 34px; font-size: 26px; line-height: 39px; text-align: center; position: absolute; border-radius: 100%; background-color: #80A093; -webkit-transform: translateY(-50%); -ms-transform: translateY(-50%); transform: translateY(-50%);"><i class="fal fa-times"></i></a>' +
                        
                        '</div>' +
                        '</div>';
                });
            } else {
                cartItemsHtml += '<div class="cart-bar__item">Giỏ của bạn đang trống!</div>';
            }

            // Thêm các phần tử sản phẩm vào div có class "cart-bar__lists"
            $('#listCart').html(cartItemsHtml);

            // Cập nhật tổng cộng
            $('.cart-bar__subtotal span:last-child').text(totalPrice + ' đ');
        },
        error: function (xhr, status, error) {
            console.error(xhr.responseText);
        }
    });
}

function removeFromCart(productId) {
    $.ajax({
        url: '/Cart/RemoveFromCart',
        type: 'POST',
        data: { productId: productId },
        success: function (response) {
            console.log("Product removed from cart successfully");
           
            updateCartContent();
            updateCartCount();
        },
        error: function (xhr, textStatus, errorThrown) {
            console.error('Error: ' + errorThrown);
        }
    });
}
function updateCartCount() {
    $.ajax({
        url: '/Cart/GetCart',
        type: 'GET',
        success: function (data) {
            // console.log(data.cartCount)
            document.getElementById("txtCart").innerHTML = data.cartCount;
            document.getElementById("txtCartRe").innerHTML = data.cartCount;
            document.getElementById("txtCartPopup").innerHTML = data.cartCount;
        },
        error: function (xhr, status, error) {
            console.error(xhr.responseText);
        }
    });
}