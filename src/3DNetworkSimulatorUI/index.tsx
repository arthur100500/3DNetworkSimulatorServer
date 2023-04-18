
import ReactDOM from 'react-dom/client';
import './index.css';
import MainPage from './components/MainPage';
import GamePage from './components/GamePage';
import {
  BrowserRouter,
  Route,
  Routes,
} from 'react-router-dom';

const root = ReactDOM.createRoot(
  document.getElementById('root') as HTMLElement
);
root.render(
  <BrowserRouter>
    <Routes>
      <Route path="/game" element={<GamePage />} />
      <Route path="/" element={<MainPage />} />
    </Routes>
  </BrowserRouter>
);
