ko.punches.enableAll();

function Model() {
    var self = this;
    self.loaded = ko.observable(true);
    self.address = ko.observable();
    self.result = ko.observable();
    self.inProgress = ko.observable(false);
    self.captcha = ko.observable();
    self.disableSubmit = ko.computed(function () {
        return self.inProgress() || $.trim(self.address()).length != 34 || self.captcha() == null;
    });
    self.onSendClick = function () {
        self.inProgress(true);
        self.result(null);
        $.post('api/Faucet/SendCoin', { address: self.address(), captcha: grecaptcha.getResponse() })
            .done(function (data) {
                self.result({ success: true, transactionId: data.transactionId });
                self.address('');
            }).fail(function (resp) {
                var data = resp.responseJSON;
                var errorMessage = data && data.errorMessage ? resp.errorMessage : "Sorry, an error occured. Please try again later.";
                self.result({ success: false, errorMessage: errorMessage });
            }).always(function () {
                self.captcha(null);
                grecaptcha.reset();
                self.inProgress(false);
            });
    };

}

var model = new Model();
ko.applyBindings(model);

function captcha(param) {
    model.captcha(param);
}