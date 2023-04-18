import React from "react";
import UnityContainer from "../UnityContainer"
import './index.css';

const GamePage: React.FC = () => (
    <>
        <h1> Game Page </h1>
        <a href="/"> Back </a>
        <div id="unity-game">
            {<UnityContainer />}
        </div>
    </>
);

export default GamePage