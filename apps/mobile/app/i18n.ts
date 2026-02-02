import translations from './translations.json';

let locale = 'en';

export function setLocale(l: string) {
  locale = l;
}

export function t(key: string) {
  const entry = (translations as any)[key];
  if (!entry) return key;
  return entry[locale] ?? entry['en'] ?? Object.values(entry)[0] ?? key;
}

export function currentLocale() {
  return locale;
}
