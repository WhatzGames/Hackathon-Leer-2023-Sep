import {cn} from '@/lib/cn';
import {Button} from '@/components/Button';
import {useGameStore} from '@/app/game-store';

export function StartScreen() {
  const startGame = useGameStore(state => state.startGame);

  return (
    <div className={
      'absolute left-0 top-0 bg-slate-800/60 z-30 backdrop-blur w-screen h-screen flex justify-center items-center'
    }>
      <div className={'flex flex-col items-center'}>
        <h1 className={cn(
          'text-8xl italic -rotate-6 font-bold animate-pulse mb-12'
        )}>
          Ready?
        </h1>
        <Button className={'mt-28 text-3xl py-6 px-12 w-fit hover:animate-ping'} onClick={startGame}>
          Start
        </Button>
      </div>
      <div className={'absolute -z-10 blur-3xl bg-gradient-to-br from-cyan-400 to-amber-400 rounded-full'}
           style={{width: 600, height: 600}}>
      </div>
    </div>
  );
}