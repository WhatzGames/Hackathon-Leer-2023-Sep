'use client';

import {useGameStore} from '@/app/game-store';
import ProgressBar from '@/app/ProgressBar';
import EndScreen from '@/app/EndScreen';
import Board from '@/app/Board';
import {Player} from '@/app/Player';

type GameProps = {
  view?: boolean;
  onNewGame?: () => void;
}

export default function Game({view = false, onNewGame}: GameProps) {
  const game = useGameStore(state => state.game);
  const players = useGameStore(state => state.players);

  return (
    <>
      <EndScreen onNewGame={onNewGame}/>
      {!view &&
        <>
          <h1 className={'text-3xl text-center mb-12'}>JAckathon - Meta Tic-Tac-Toe</h1>
        </>
      }

      <div className="flex justify-around">
        {players.map((player) => (
          <h2 key={player.id} className={'text-2xl text-center mb-4 flex items-center gap-4'}>
            <Player>{player.symbol}</Player>
            <span>{player.name || player.id}</span>
          </h2>
        ))}
      </div>

      <div className={'flex justify-center'}>
        {!view &&
          <div id={'progress-bar'} className={'mr-8'}>
            <ProgressBar/>
          </div>
        }
        <div className={'grid grid-cols-3 grid-rows-3 gap-3'}>
          {game.map((board, index) => (
            <Board key={board.id} boardNo={index} fields={board.fields} view={view}/>
          ))}
        </div>
      </div>
    </>
  );
}
