import * as React from 'react';
import 'isomorphic-fetch';

export interface ContinuumStatusState {
    status: number;
    name: string;
    url: string;
    group: string;
    pipelineName: string;
    project: string;
    instanceId: string;
    ignored: boolean;
}

export class ContinuumStatus extends React.Component<{ status: ContinuumStatusState[], onCtmIgnore: (id: string) => any }, ContinuumStatusState[]> {
    constructor() {
        super();
        this.state = null;
    }

    public render() {
        return <div className="status">
            <h2 className="continuum">Continuum</h2>
            {Object.keys(this.props.status).map(key => this.renderStatus(key, this.props.status[key]))}
        </div>;
    }
    /*{this.props.status.map(status => ContinuumStatus.renderStatus(status))}*/

    private static onClick(e) {
        console.log( "got it! "+e)
    }

    private static mapCtmStatus(status: Number) {
        switch (status) {
            case 0: // ContinuumStatus.CtmStatus.Staged:
                return "gray";
            case 1: // ContinuumStatus.CtmStatus.Running:
                return "blue";
            case 2: // ContinuumStatus.CtmStatus.Ok:
                return "green";
            case 3: // ContinuumStatus.CtmStatus.Failed:
                return "red";
            case 4: // ContinuumStatus.CtmStatus.Canceled:
                return "gray";
            case 5: // ContinuumStatus.CtmStatus.notRunYet:
                return "gray";
            case 6: // ContinuumStatus.CtmStatus.pending:
                return "lightblue";
            default:
                return "black";
        }
    }

    private renderStatus(key: string, status: ContinuumStatusState) {
        return <div key={key}>
            <svg className="SS" viewBox="0 0 100 100">
                <defs>
                    <radialGradient id={key} fx="30%" fy="30%">
                        <stop offset="10%" stopColor="lightgray" />
                        <stop offset="95%" stopColor={ContinuumStatus.mapCtmStatus(status.status)} />
                    </radialGradient>
                </defs>
                <a onClick={() => this.props.onCtmIgnore(status.instanceId)} target="_blank">
                    <circle className="SS" cx="50" cy="50" r="48" fill={"url(#" + key + ")"}>
                        <title>{status.name}</title>
                    </circle>
                </a>
            </svg>&nbsp;
            <a className="SS" href={status.url} color={ContinuumStatus.mapCtmStatus(status.status)} target="_blank">{status.name} {status.pipelineName} {status.group}</a>
        </div>;
    }
}
