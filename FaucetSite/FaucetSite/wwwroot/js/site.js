ko.punches.enableAll();
function Model() {
    var self = this;
    self.loaded = ko.observable(true);
    self.balance = { balance: ko.observable(0), returnAddress: ko.observable('') };
    self.address = ko.observable();
    self.transactionId = ko.observable();
    self.inProgress = ko.observable(false);
    self.disableSubmit = ko.computed(function () {
        return self.inProgress() || $.trim(self.address()).length != 34;
    });
    self.confirmation = ko.observable(0);
    $.getJSON('api/Faucet/GetBalance').done(function (result) {
        self.balance.balance(result.balance);
        self.balance.returnAddress(result.returnAddress);
        self.loaded(true);
    });

    self.onSendClick = function () {
        self.inProgress(true);
        $.ajax({
            url: 'api/Faucet/SendCoin', 
            method:'POST',
            data: JSON.stringify({ address: self.address() }),
            dataType: 'json',
            contentType: 'application/json; charset=utf-8'
        }).done(function (result) {
            self.transactionId(result.transactionId);
            self.address('');
            }).always(function () {
                self.inProgress(false);
            });
    }
}

var model = new Model();
ko.applyBindings(model);