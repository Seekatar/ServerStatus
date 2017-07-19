import * as React from 'react';
import 'isomorphic-fetch';

export interface ZabbixStatusState {
    priority: number;
    name: string;
    url: string;
}

export class ZabbixStatus extends React.Component<{ status: ZabbixStatusState[] }, ZabbixStatusState[]> {
    constructor() {
        super();
        this.state = null;

    }

    public render() {
        return <div className="status">
            <h2 className="zabbix">Zabbix</h2>
            {Object.keys(this.props.status).map(key => ZabbixStatus.renderStatus(key, this.props.status[key]))}
        </div>;
    }

    private static mapZabbixPriority(priority: Number) {
        switch (priority) {
            case 0:
                return "green";
            case 1:
                return "blue";
            case 2:
                return "yellow";
            case 3:
                return "orange";
            case 4:
                return "darkred";
            case 5:
                return "red";
            default:
                return "black";
        }
    }

    private renderKey(key: string) {
        // return <div>key is {key} status is {this.props.status[key].url} </div>
        return ZabbixStatus.renderStatus(key, this.props.status[key])
    }

    private static renderStatus(key: string, status: ZabbixStatusState) {
        return <div key={key} style={{textAlign:"right"}}>
            <a className="SS" href={status.url} color={ZabbixStatus.mapZabbixPriority(status.priority)} target="_blank">{status.name}</a>&nbsp;
            <svg className="SS" viewBox="0 0 100 100">
                <defs>
                    <radialGradient id={key+"z"} fx="30%" fy="30%">
                        <stop offset="10%" stopColor="lightgray" />
                        <stop offset="95%" stopColor={ZabbixStatus.mapZabbixPriority(status.priority)} />
                    </radialGradient>
                </defs>
                <a href={status.url} target="_blank">
                    <circle className="SS" cx="50" cy ="50" r="48" fill={"url(#" + key + "z)"}>
                        <title>{status.name}</title>
                    </circle>
                </a>
            </svg>
        </div>;
    }
}
