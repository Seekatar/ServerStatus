import * as React from 'react';
import 'isomorphic-fetch';

import { ZabbixStatusState, ZabbixStatus } from './ZabbixStatus';
import { ContinuumStatusState, ContinuumStatus } from './ContinuumStatus';


interface StatusResponse {
    item1: string;
    item2: ContinuumStatusState[];
    item3: ZabbixStatusState[];
    loading: boolean;
    elapsedSeconds: number;
}

export class ServerStatus extends React.Component<{}, StatusResponse> {
    private interval = null;
    constructor() {
        super();
        this.state = {
            item1: "",
            item2: [],
            item3: [],
            loading: true,
            elapsedSeconds: 0
        };

        this.fetchData();
        this.tick = this.tick.bind(this); // must bind tick to this, otherwise 'this' is Window when tick called.
    }

    private rowCount() {
        var match = location.search.match("count=([0-9]+)");
        var count = "20";
        if ( match != null && match.length == 2 )
        {
            count = match[1];
        }
        return count;
    }

    private fetchData() {

        fetch('api/status?count='+this.rowCount())
            .then(response => response.json() as Promise<StatusResponse>)
            .then(data => {
                this.setState({ item1: (new Date(data.item1)).toLocaleTimeString(), item2: data.item2, item3: data.item3, loading: false });
            });
    }

    public tick() {
        this.fetchData();
    }

    public componentDidMount() {
        this.interval = setInterval(this.tick, 5000);
    }

    public componentWillUnmount() {
        clearInterval(this.interval);
    }

    private anyRed()
    {
        var count = parseInt(this.rowCount());

        // ctm 3 = red, zabbix 4,5 = red
        return this.state.item2.slice(0,count).some( o => { return o.status == 3; }) || this.state.item3.slice(0,count).some( o => { return o.priority == 4 || o.priority == 5; });
    }

    public componentDidUpdate() {

        var link = document.createElement('link')
        var oldLink = document.getElementById('dynamic-favicon');
        link.id = 'dynamic-favicon';
        link.rel = 'shortcut icon';
        link.href = this.anyRed() ? "red.ico" : "favicon.ico";
        if (oldLink) {
            document.head.removeChild(oldLink);
        }
        document.head.appendChild(link);
    }

    public render() {
        let contents = this.state.loading
            ? <p><em>Loading...</em></p>
            : this.renderStatus();

        return <span>
            <h1 className="SS">Server Status</h1>
            {contents}
        </span>;
    }

    private renderStatus() {
        var status = this.state;
        return <span>
            <div className="asof">as of {status.item1}</div>
            <div id="data">
                <ContinuumStatus status={status.item2} />
                <div id="separator">&nbsp;&nbsp;</div>
                <ZabbixStatus status={status.item3} />
            </div>
        </span>;
    }
}
