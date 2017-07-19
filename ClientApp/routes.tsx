import * as React from 'react';
import { Route } from 'react-router-dom';
import { Layout } from './components/Layout';
import { ServerStatus } from './components/ServerStatus';

export const routes = <Layout>
    <Route exact path='/' component={ ServerStatus } />
</Layout>;
