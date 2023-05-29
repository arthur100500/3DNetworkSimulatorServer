import React from "react";
import UnityContainer from "../UnityContainer"
import './index.css';

const GamePage: React.FC = () => (
    <>
        <div id="main-div">
            <h1> Game Page </h1>
            <div id="unity-game">
                {<UnityContainer />}
            </div>
            <a href="/"> Back </a>
        </div>
    </>
);

export default GamePage