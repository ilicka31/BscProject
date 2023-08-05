import React from 'react';
import ReactDOM from 'react-dom/client';
import './index.css';
import { BrowserRouter as Router } from 'react-router-dom';
import { Provider } from 'react-redux';
import store from "./Store/store";
import { ToastContainer } from 'react-toastify';
import App from './App';
import { GoogleOAuthProvider } from '@react-oauth/google';
import { createTheme, ThemeProvider } from '@mui/material/styles';

const defaultTheme = createTheme({
  palette: {
    mode: 'dark',
  },
});

const root = ReactDOM.createRoot(document.getElementById('root'));
root.render(
  <ThemeProvider theme={defaultTheme}>
    <Router>
      <GoogleOAuthProvider clientId={process.env.REACT_APP_GOOGLE_CLIENT_ID || ""}>
      <Provider store={store}>
        <App/>
      </Provider>
      </GoogleOAuthProvider>
      <ToastContainer/>
    </Router>
    </ThemeProvider>
  
);

