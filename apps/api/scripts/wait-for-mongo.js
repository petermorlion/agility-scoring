// Simple wait-for-mongo script used by `npm run dev` to wait until Mongo is reachable.
const { MongoClient } = require('mongodb');

const uri = process.env.MONGODB_URI || 'mongodb://localhost:27017';
const timeoutMs = parseInt(process.env.WAIT_TIMEOUT_MS || '300000', 10); // default 5 minutes

async function waitForMongo() {
  const start = Date.now();
  while (Date.now() - start < timeoutMs) {
    try {
      const client = new MongoClient(uri);
      await client.connect();
      await client.db('admin').command({ ping: 1 });
      await client.close();
      console.log('\n✅ MongoDB is ready at', uri);
      process.exit(0);
    } catch (err) {
      process.stdout.write('.');
      await new Promise((r) => setTimeout(r, 1000));
    }
  }
  console.error('\n⚠️ Timed out waiting for MongoDB at', uri);
  process.exit(1);
}

waitForMongo();
