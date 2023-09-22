'use client';

import {useGameStore} from '@/app/game-store';
import ProgressBar from '@/app/ProgressBar';
import EndScreen from '@/app/EndScreen';
import Board from '@/app/Board';
import {PlayedGame} from '@/app/types';
import {useEffect} from 'react';

export default function Game({playedGame}: {playedGame?: PlayedGame}) {
  const game = useGameStore(state => state.game);
  const loadGame = useGameStore(state => state.load);
  const player = useGameStore(state => state.activePlayer);

  useEffect(() => {
    if (playedGame) {
      loadGame(playedGame);
    }
  }, []);

  console.log(game);

  return (
    <>
      <EndScreen/>
      {!playedGame &&
        <>
          <h1 className={'text-3xl text-center'}>JAckathon - Meta Tic-Tac-Toe</h1>
          <h3 className={'text-xl text-center mb-12'}>Game ID: ABC</h3>

          <h2 className={'text-2xl text-center mb-4'}>Spieler {player.toUpperCase()}, du bist dran!</h2>
        </>
      }

      <div className={'flex'}>
        <div id={'progress-bar'} className={'mr-8'}>
          <ProgressBar/>
        </div>
        <div className={'grid grid-cols-3 grid-rows-3 gap-3'}>
          {game.map((board, index) => <Board key={board.id} boardNo={index} fields={board.fields}/>)}
        </div>
      </div>
    </>
  );
}
