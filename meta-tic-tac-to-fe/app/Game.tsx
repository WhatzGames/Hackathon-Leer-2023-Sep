'use client';

import {useGameStore} from '@/app/game-store';
import ProgressBar from '@/app/ProgressBar';
import EndScreen from '@/app/EndScreen';
import Board from '@/app/Board';
import {Player} from '@/app/Player';
import {cn} from '@/lib/cn';

type GameProps = {
  view?: boolean;
  onNewGame?: () => void;
}

export default function Game({view = false, onNewGame}: GameProps) {
  const game = useGameStore(state => state.game);
  const activePlayer = useGameStore(state => state.activePlayer);
  const players = useGameStore(state => state.players);

  return (
    <>
      <EndScreen onNewGame={onNewGame}/>

      <div className="flex justify-around">
        {players.map((player) => (
          <h2 key={player.id}
              className={cn(
                'text-2xl text-center mb-4 flex items-center gap-4 transition-transform',
                activePlayer === player.id ? 'scale-125' : 'scale-75 opacity-50'
              )}>
            <Player>{player.symbol}</Player>
            <span>{player.name || player.id}</span>
          </h2>
        ))}
      </div>

      {!view && <ProgressBar className={'mb-4'}/>}

      <div className={'grid grid-cols-3 grid-rows-3 gap-3'}>
        {game.map((board, index) => (
          <Board key={board.id} boardNo={index} fields={board.fields} view={view}/>
        ))}
      </div>
    </>
  );
}
