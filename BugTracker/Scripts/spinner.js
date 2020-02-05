$(window).on('load', function () {
    $(".loadingDiv-parent").fadeOut(500);
});
$(window).on('beforeunload', function () {
    $(".loadingDiv-parent").fadeIn(500);
});