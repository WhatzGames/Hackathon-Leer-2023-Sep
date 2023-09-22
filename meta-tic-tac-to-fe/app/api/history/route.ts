import {NextResponse} from 'next/server';
import {readdirSync, readFileSync, statSync} from 'fs';
import path from 'path';
import {PlayedGame} from '@/app/types';

export async function GET() {
  const dirRelativeToPublicDir = 'history';
  const dir = path.resolve('./public', dirRelativeToPublicDir);

  const files = readdirSync(dir);
  const filePaths = files.map(name => {
    const filePath = path.resolve('./public', dirRelativeToPublicDir, name);

    return {
      filePath,
      time: statSync(filePath).mtime.getTime()
    };
  })
    .sort((a, b) => a.time - b.time)
    .map(file => file.filePath);

  const games: PlayedGame[] = [];
  filePaths.forEach((filePath) => {
    let data = readFileSync(filePath, 'utf-8');

    try {
      const game = JSON.parse(data.trim());
      games.push(game);
    } catch (e) {
      console.error('could not parse json', e);
    }
  });

  return NextResponse.json(games);
}
