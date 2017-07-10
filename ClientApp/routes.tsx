import * as React from 'react';
import { Route } from 'react-router-dom';
import { Layout } from './components/Layout';
import { V1Status } from './components/V1Status';

export const routes = <Layout>
    <Route exact path='/' component={ V1Status } />
</Layout>;
