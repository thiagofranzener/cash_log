import React from 'react';
import { createBottomTabNavigator } from '@react-navigation/bottom-tabs';

import ConsultaDespesas from '../Screens/ConsultaDespesas';
import CadastroDespesas from '../Screens/CadastroDespesas';
import AprovarDespesas from '../Screens/AprovarDespesas';
import PerfilUsuario from '../Screens/PerfilUsuario';

import CustomTabBar from '../Components/CustomTabBar';

const Tab = createBottomTabNavigator();

export default () => (
    <Tab.Navigator tabBar={props=><CustomTabBar {...props} />}>
        <Tab.Screen name="ConsultaDespesas" component={ConsultaDespesas} />
        <Tab.Screen name="CadastroDespesas" component={CadastroDespesas} />
        <Tab.Screen name="AprovarDespesas" component={AprovarDespesas} />
        <Tab.Screen name="PerfilUsuario" component={PerfilUsuario} />
    </Tab.Navigator>
);