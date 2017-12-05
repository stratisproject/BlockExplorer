ko.punches.enableAll();

function Model() {
    var self = this;
    self.loaded = ko.observable(true);
    self.address = ko.observable();
    self.success = ko.observable();
    self.inProgress = ko.observable(false);
    self.captcha = ko.observable();
    self.errorMessage = ko.observable();
    self.disableSubmit = ko.computed(function () {
        return self.inProgress() || $.trim(self.address()).length != 34 || self.captcha() == null;
    });
    self.onSendClick = function () {
        self.inProgress(true);
        self.errorMessage(null);
        self.success(null);
        $.post('api/Faucet/SendCoin', { address: self.address(), captcha: grecaptcha.getResponse() })
            .done(function () {
                self.success(true);
                self.address('');
            }).fail(function (resp) {
                var data = resp.responseJSON;
                if (data && data.errorMessage) {
                    self.errorMessage(data.errorMessage);
                } else {
                    self.errorMessage("Sorry, an error occured. Please try again later.");
                }
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