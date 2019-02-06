$('.nav-burger').on('click', function (e) {
    e.preventDefault;
    $(this).toggleClass('burger-active');
});

$(window).innerWidth(function () {
    var w = $(window).width();
    if (w <= 570) {
        $('div.container').toggleClass('container-fluid');
        $('div.container').toggleClass('container');
    }
    if (w >= 570) {
        $('div.container-fluid').toggleClass('container');
        $('div.container-fluid').toggleClass('container-fluid');
    }
});

$(window).on('resize', function () {
    var w = $(window).width();
    if (w <= 570) {
        $('table').attr('cellpadding', '5');
        $('div.container').toggleClass('container-fluid');
        $('div.container').toggleClass('container');
    }
    if (w >= 570) {
        $('div.container-fluid').toggleClass('container');
        $('div.container-fluid').toggleClass('container-fluid');
    }
});