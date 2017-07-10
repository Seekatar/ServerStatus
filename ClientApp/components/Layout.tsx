import * as React from 'react';
import { V1Status } from './V1Status';

export interface LayoutProps {
    children?: React.ReactNode;
}

export class Layout extends React.Component<LayoutProps, {}> {
    public render() {
        return <div className='container-fluid' style={{padding:0}}>
            <div>
                <V1Status/>
            </div>
        </div>;
    }
}
