'use client';

import {useEffect} from 'react';
import {cn} from '@/lib/cn';
import {useGameStore} from '@/app/game-store';
import {useProgressStore} from '@/app/progress-store';

export default function Game() {
  const game = useGameStore(state => state.game);
  const player = useGameStore(state => state.player);

  return (
    <main>
      <EndScreen/>
      <h1 className={'text-3xl text-center'}>JAckathon - Meta Tic-Tac-Toe</h1>
      <h3 className={'text-xl text-center mb-12'}>Game ID: ABC</h3>

      <h2 className={'text-2xl text-center mb-4'}>Spieler {player.toUpperCase()}, du bist dran!</h2>

      <div className={'flex'}>
        <div id={'progress-bar'} className={'mr-8'}>
          <ProgressBar/>
        </div>
        <div className={'grid grid-cols-3 grid-rows-3 gap-3'}>
          {game.map((board, index) => <Board key={index} boardNo={index} fields={board.fields}/>)}
        </div>
      </div>
    </main>
  );
}

const EndScreen = () => {
  const progress = useProgressStore(state => state.progress);
  const player = useGameStore(state => state.player);
  const newGame = useGameStore(state => state.newGame);
  const resetProgress = useProgressStore(state => state.reset);

  if (progress > 0) {
    return null;
  }

  const handleClick = () => {
    newGame();
    resetProgress();
  };

  return (
    <div className={
      'absolute left-0 top-0 bg-white/60 z-30 backdrop-blur w-screen h-screen flex justify-center items-center'
    }>
      <div className={'flex flex-col items-center'}>
        <h1 className={'text-6xl italic -rotate-6 font-bold animate-pulse'}>Spieler {player} gewonnen!</h1>
        <button className={'mt-28 text-3xl bg-white rounded-xl py-6 px-12 w-fit hover:animate-ping'}
                onClick={handleClick}>
          Neues Spiel
        </button>
      </div>
      <div className={'absolute -z-10 blur-3xl bg-gradient-to-br from-cyan-400 to-amber-400 rounded-full'}
           style={{width: 400, height: 400}}>
      </div>
    </div>
  );
};

const ProgressBar = () => {
  const player = useGameStore(state => state.player);
  const {progress, tick, reset} = useProgressStore();

  /*useEffect(() => {
    const interval = setInterval(() => tick(), 50);
    return () => {
      clearInterval(interval);
    };
  },  []);*/

  useEffect(() => {
    reset();
  }, [player]);

  return (
    <div className={'relative bg-gray-200 mx-2 rounded-lg h-full'} style={{width: 12}}>
      <div className={'absolute bottom-0 rounded-lg bg-red-300 z-10'} style={{width: 12, height: `${progress}%`}}></div>
    </div>
  );
};

type BoardProps = {
  boardNo: number;
  fields: (string | null)[];
}

const Board = ({boardNo, fields}: BoardProps) => {
  const activeBoard = useGameStore(state => state.activeBoard);
  const finishedBoards = useGameStore(state => state.finishedBoards);
  const winner = finishedBoards[boardNo];

  return (
    <div className={cn(
      'relative transition-all',
      activeBoard !== boardNo ? (activeBoard === null && !winner ? 'scale-100' : 'scale-90') : 'scale-110'
    )}>
      {(winner || (activeBoard !== null && activeBoard !== boardNo)) &&
        <div className={cn(
          'text-6xl text-red-white absolute bg-slate-700/60 w-full h-full flex justify-center',
          'items-center shadow-xl'
        )}>
          <span>{winner}</span>
        </div>
      }
      <div className={cn(
        'grid grid-cols-3 grid-rows-3 gap-1',
        winner && 'border-red-950'
      )}>
        {fields.map((field, index) => <Field key={index} board={boardNo} field={index}>{field}</Field>)}
      </div>
    </div>
  );
};

type FieldProps = {
  board: number;
  field: number;
  width?: number;
  height?: number;
}

const Field = ({board, field, width = 80, height = 80, children}: React.PropsWithChildren<FieldProps>) => {
  const setField = useGameStore(state => state.setField);

  const handleClick = () => setField(board, field);

  return (
    <button className={'border border-cyan-400 hover:bg-cyan-300 hover:animate-pulse'}
            onClick={handleClick}
            style={{width, height}}>
      {children}
    </button>
  );
};
