ko.punches.enableAll();
function Model() {
    var self = this;
    self.loaded = ko.observable(true);
    self.balance = { balance: ko.observable(0), returnAddress: ko.observable('') };
    self.address = ko.observable();
    self.transaction = ko.observable();
    self.valid = ko.computed(function () {
        return $.trim(self.address()).length == 34;
    });
    self.confirmation = ko.observable(0);
    $.getJSON('api/Faucet/GetBalance').done(function (result) {
        self.balance.balance(result.balance);
        self.balance.returnAddress(result.returnAddress);
        self.loaded(true);
    });

    self.onSendClick = function () {
        $.post({
            url: 'api/Faucet/SendCoin', 
            data: { address: self.address() },
            dataType: 'json',
            contentType: 'application/json; charset=utf-8'
        }).done(function (result) {
            self.transaction(result);
            self.address('');
        });
    }
}

var model = new Model();
ko.applyBindings(model);