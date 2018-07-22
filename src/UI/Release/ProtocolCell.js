var Backgrid = require('backgrid');

module.exports = Backgrid.Cell.extend({
    className: 'protocol-cell',

    render: function () {
        var protocol = (this.model.get('protocol') || 'Unknown').toLowerCase();

        if (protocol) {
            var label = '??';

            switch (protocol) {
                case 'torrent':
                    label = 'torrent';
                    break;
                case 'usenet':
                    label = 'usenet';
                    break;
                case 'jdownloader':
                    label = 'dlc';
                    break;
                default:
                    break;
            }
            this.$el.html('<div class="label label-default protocol-{0}" title="{0}">{1}</div>'.format(protocol, label));
        }

        this.delegateEvents();

        return this;
    }
});
