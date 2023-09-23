import {useProgressStore} from '@/app/progress-store';
import {useGameStore} from '@/app/game-store';
import {Button} from '@/components/Button';
import {cn} from '@/lib/cn';
import {Play} from 'lucide-react';
import {Player} from '@/app/Player';
import {Simulate} from 'react-dom/test-utils';
import play = Simulate.play;

export default function EndScreen({onNewGame}: {onNewGame?: () => void}) {
  const progress = useProgressStore(state => state.progress);
  const winner = useGameStore(state => state.winner);
  const players = useGameStore(state => state.players);
  const newGame = useGameStore(state => state.newGame);
  const resetProgress = useProgressStore(state => state.reset);

  if (!winner) {
    return null;
  }

  const handleClick = () => {
    newGame();
    resetProgress();

    if (onNewGame) {
      onNewGame();
    }
  };

  return (
    <div className={
      'absolute left-0 top-0 bg-slate-800/60 z-30 backdrop-blur w-screen h-screen flex justify-center items-center'
    }>
      <div className={'flex flex-col items-center'}>
        <h1 className={cn(
          'text-8xl italic -rotate-6 font-bold animate-pulse mb-12'
        )}>
          {players.find(p => p.symbol === winner)?.name}
        </h1>
        <h3 className={'text-4xl'}>hat gewonnen!</h3>
        <Button className={'mt-28 text-3xl py-6 px-12 w-fit hover:animate-ping'}
                onClick={handleClick}>
          Neues Spiel
        </Button>
      </div>
      <div className={'absolute -z-10 blur-3xl bg-gradient-to-br from-cyan-400 to-amber-400 rounded-full'}
           style={{width: 600, height: 600}}>
      </div>
    </div>
  );
}
