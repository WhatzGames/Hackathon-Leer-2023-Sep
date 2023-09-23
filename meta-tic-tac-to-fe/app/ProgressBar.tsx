import {useGameStore} from '@/app/game-store';
import {useProgressStore} from '@/app/progress-store';
import {useEffect} from 'react';
import {cn} from '@/lib/cn';

export default function ProgressBar({className, ...props}: React.HTMLAttributes<HTMLDivElement>) {
  const player = useGameStore(state => state.activePlayer);
  const isRunning = useGameStore(state => state.isRunning);
  const endGame = useGameStore(state => state.endGame);
  const {progress, tick, reset} = useProgressStore();

  useEffect(() => {
    let interval: ReturnType<typeof setInterval>;

    if (isRunning) {
      interval = setInterval(() => tick(), 50);
    }

    return () => {
      if (interval) {
        clearInterval(interval);
      }
    };
  },  [isRunning]);

  useEffect(() => {
    if (progress <= 0) {
      endGame();
    }
  }, [progress]);

  useEffect(() => {
    reset();
  }, [player]);

  return (
    <div className={cn('relative bg-gray-200 rounded-lg w-full', className)}
         style={{height: 12}}
         {...props}>
      <div className={'absolute bottom-0 rounded-lg bg-red-300 z-10'} style={{height: 12, width: `${progress}%`}}></div>
    </div>
  );
};
