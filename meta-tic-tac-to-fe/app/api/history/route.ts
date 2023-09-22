import {NextResponse} from 'next/server';
import * as fs from 'node:fs';
import path from 'path';

export function GET() {
  const dirRelativeToPublicDir = 'history';
  const dir = path.resolve('./public', dirRelativeToPublicDir);

  const fileNames = fs.readdirSync(dir);
  const files = fileNames.map(name => path.join('/', dirRelativeToPublicDir, name));

  return NextResponse.json(files);
}
