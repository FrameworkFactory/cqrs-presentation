
if (typeof Object.assign != 'function') {
  Object.assign = function(target, varArgs) {
    'use strict';
    if (target == null) {
      throw new TypeError('Cannot convert undefined or null to object');
    }

    var to = Object(target);

    for (var index = 1; index < arguments.length; index++) {
      var nextSource = arguments[index];

      if (nextSource != null) { // Skip over if undefined or null
        for (var nextKey in nextSource) {
          // Avoid bugs when hasOwnProperty is shadowed
          if (Object.prototype.hasOwnProperty.call(nextSource, nextKey)) {
            to[nextKey] = nextSource[nextKey];
          }
        }
      }
    }
    return to;
  };
}

export function isUndefined(value) {
  return typeof value === 'undefined'
}

export function isDefined(value) {
  return typeof value !== 'undefined'
}

export function isObject(value) {
  return value !== null && typeof value === 'object'
}

export function isString(value) {
  return typeof value === 'string'
}

export function isNumber(value) {
  return typeof value === 'number'
}

export function isFunction(value) {
  return typeof value === 'function'
}


export function camelCase(name) {
  return name.replace(/([\:\-\_]+(.))/g, function (_, separator, letter, offset) {
    return offset ? letter.toUpperCase() : letter;
  });
}

export function isPresent(value) {
  if (isString(value)) {
    return value.length > 0 && value.trim().length > 0
  }
}

export function isMissing(value) {
  return !isPresent(value)
}

export function objectExtend(a, b) {

  // Don't touch 'null' or 'undefined' objects.
  if (a == null || b == null) {
    return a;
  }

  Object.keys(b).forEach(function (key) {
    if (Object.prototype.toString.call(b[key]) == '[object Object]') {
      if (Object.prototype.toString.call(a[key]) != '[object Object]') {
        a[key] = b[key];
      } else {
        a[key] = objectExtend(a[key], b[key]);
      }
    } else {
      a[key] = b[key];
    }
  });

  return a;
};


export function decodeBase64(str) {
  let buffer;
  if (typeof module !== 'undefined' && module.exports) {
    try {
      buffer = require('buffer').Buffer;
    } catch (err) {
      // noop
    }
  }

  let fromCharCode = String.fromCharCode;

  let re_btou = new RegExp([
    '[\xC0-\xDF][\x80-\xBF]',
    '[\xE0-\xEF][\x80-\xBF]{2}',
    '[\xF0-\xF7][\x80-\xBF]{3}'
  ].join('|'), 'g');

  let cb_btou = function (cccc) {
    switch (cccc.length) {
      case 4:
        let cp = ((0x07 & cccc.charCodeAt(0)) << 18)
          | ((0x3f & cccc.charCodeAt(1)) << 12)
          | ((0x3f & cccc.charCodeAt(2)) << 6)
          | (0x3f & cccc.charCodeAt(3));
        let offset = cp - 0x10000;
        return (fromCharCode((offset >>> 10) + 0xD800)
        + fromCharCode((offset & 0x3FF) + 0xDC00));
      case 3:
        return fromCharCode(
          ((0x0f & cccc.charCodeAt(0)) << 12)
          | ((0x3f & cccc.charCodeAt(1)) << 6)
          | (0x3f & cccc.charCodeAt(2))
        );
      default:
        return fromCharCode(
          ((0x1f & cccc.charCodeAt(0)) << 6)
          | (0x3f & cccc.charCodeAt(1))
        );
    }
  };

  let btou = function (b) {
    return b.replace(re_btou, cb_btou);
  };

  let _decode = buffer ? function (a) {
    return (a.constructor === buffer.constructor
      ? a : new buffer(a, 'base64')).toString();
  }
    : function (a) {
    return btou(atob(a));
  };

  return _decode(
    String(str).replace(/[-_]/g, function (m0) {
      return m0 === '-' ? '+' : '/';
    })
      .replace(/[^A-Za-z0-9\+\/]/g, '')
  );
}

export function getFullUrlPath(location) {
  const isHttps = location.protocol === 'https:';
  return location.protocol + '//' + location.hostname + ':' + (location.port || (isHttps ? '443' : '80'))
}


