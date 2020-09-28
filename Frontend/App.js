import React from 'react';
import {
  Container,
  Title,
  Input,
  Button,
  TextButton,
  FooterButtons,
} from './styled';

const App = () => {
  return <Container>
    <Title>Cashlog</Title>
    <Input placeholder="E-mail" />
    <Input placeholder="Senha" />
    <Button>
      <TextButton>ENTRAR</TextButton>
    </Button>
  </Container>
};

export default App;