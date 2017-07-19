import * as React from 'react';
import { ServerStatus } from './ServerStatus';

export interface LayoutProps {
    children?: React.ReactNode;
}

export class Layout extends React.Component<LayoutProps, {}> {
    public render() {
        return <div className='container-fluid' style={{padding:0}}>
            <div>
                <ServerStatus/>
            </div>
        </div>;
    }
}
