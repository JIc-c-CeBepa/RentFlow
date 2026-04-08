export function formatPhone(value) {
  const digits = value.replace(/\D/g, "");

  let normalized = digits;

  if (normalized.startsWith("8")) {
    normalized = "7" + normalized.slice(1);
  }

  if (!normalized.startsWith("7")) {
    normalized = "7" + normalized;
  }

  normalized = normalized.slice(0, 11);

  const part1 = normalized.slice(1, 4);
  const part2 = normalized.slice(4, 7);
  const part3 = normalized.slice(7, 9);
  const part4 = normalized.slice(9, 11);

  let result = "+7";

  if (part1) result += ` (${part1}`;
  if (part1.length === 3) result += `)`;
  if (part2) result += `-${part2}`;
  if (part3) result += `-${part3}`;
  if (part4) result += `-${part4}`;

  return result;
}

export function getPhoneDigits(value) {
  return value.replace(/\D/g, "");
}

export function isPhoneComplete(value) {
  const digits = getPhoneDigits(value);
  return digits.length === 11 && digits.startsWith("7");
}