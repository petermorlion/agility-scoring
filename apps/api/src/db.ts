import { MongoClient, Db } from 'mongodb';

const MONGO_URI = process.env.MONGODB_URI ?? 'mongodb://localhost:27017';
const MONGO_DB = process.env.MONGODB_DB ?? 'agility_scoring';

let client: MongoClient | null = null;
let db: Db | null = null;

export async function connectToDb() {
  if (db) return db;
  client = new MongoClient(MONGO_URI);
  await client.connect();
  db = client.db(MONGO_DB);
  console.log(`✅ Connected to MongoDB at ${MONGO_URI}/${MONGO_DB}`);
  return db;
}

export function getDb() {
  if (!db) throw new Error('Database not connected. Call connectToDb() first.');
  return db as Db;
}

export async function closeDb() {
  if (client) {
    await client.close();
    client = null;
    db = null;
  }
}
