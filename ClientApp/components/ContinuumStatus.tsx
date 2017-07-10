import * as React from 'react';
import 'isomorphic-fetch';

export interface ContinuumStatusState {
    severity: number;
    name: string;
    url: string;
}

export class ContinuumStatus extends React.Component<{ status: ContinuumStatusState[] }, ContinuumStatusState[]> {
    constructor(status: ContinuumStatusState[]) {
        super();
        this.state = null;

    }
    public status: ContinuumStatusState[];

    public render() {
        return <div className="status">
            <h2 className="continuum">Continuum</h2>
            {Object.keys(this.props.status).map(key => ContinuumStatus.renderStatus(key, this.props.status[key]))}
        </div>;
    }
    /*{this.props.status.map(status => ContinuumStatus.renderStatus(status))}*/


    private static mapCtmSeverity(severity: Number) {
        switch (severity) {
            case 0: // ContinuumStatus.CtmSeverity.Staged:
                return "gray";
            case 1: // ContinuumStatus.CtmSeverity.Running:
                return "blue";
            case 2: // ContinuumStatus.CtmSeverity.Ok:
                return "green";
            case 3: // ContinuumStatus.CtmSeverity.Failed:
                return "red";
            case 4: // ContinuumStatus.CtmSeverity.Canceled:
                return "gray";
            default:
                return "black";
        }
    }

    private static renderStatus(key: string, status: ContinuumStatusState) {
        return <div key={key}>
            <svg className="V1SS" viewBox="0 0 100 100">
                <defs>
                    <radialGradient id={key} fx="30%" fy="30%">
                        <stop offset="10%" stopColor="lightgray" />
                        <stop offset="95%" stopColor={ContinuumStatus.mapCtmSeverity(status.severity)} />
                    </radialGradient>
                </defs>
                <a href={status.url} target="_blank">
                    <circle className="V1SS" cx="50" cy="50" r="48" fill={"url(#" + key + ")"}>
                        <title>{status.name}</title>
                    </circle>
                </a>
            </svg>&nbsp;
            <a href={status.url} color={ContinuumStatus.mapCtmSeverity(status.severity)} target="_blank">{status.name}</a>
        </div>;
    }
}
